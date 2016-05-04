using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduBusFiddler
{

    class Program
    {
        static Proxy oSecureEndpoint = null;
        static string sSecureEndpointHostname = "localhost";
        static int iSecureEndpointPort = 8888;
        static Thread writeThread = new Thread(WriteToDB);
        static Queue<Fiddler.Session> QueueSessions = new Queue<Session>();
        private static void WriteToDB()
        {
            while (true)
            {
                try
                {
                    if (QueueSessions.Count > 0)
                    {
                        Session oSession = QueueSessions.Dequeue();
                        WriteToFiles(oSession);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    File.AppendAllText("d:\\Bus_log.txt", e.Message.ToString());
                    continue;
                }
            }
        }
        private static void WriteToFiles(Session dataReceived)
        {
            //oS.utilDecodeResponse();       //针对js可解析         
            if (dataReceived.oResponse.MIMEType == "text/javascript" || dataReceived.oResponse.MIMEType == "text/html")
            {
                Session iSeesion = new Session(new SessionData(dataReceived));


                while ((iSeesion.state <= SessionStates.ReadingResponse))
                {
                    continue;
                }
                // 百度地图

                if (dataReceived.fullUrl.Contains("qt=bsl"))
                {
                    #region
                    iSeesion.utilDecodeResponse();
                    string str = iSeesion.GetResponseBodyAsString();
                    str = Helper.UnicodeToGb(str);
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    BaiduBus baiduBus = js.Deserialize<BaiduBus>(str);
                    BaiduBusFiddler.BusEntities db = new BusEntities();
                    //线路
                    route_baidu rt_baidu = new route_baidu
                    {
                        describe = baiduBus.content[0].name,
                        name = baiduBus.content[0].name,
                        line_direction = baiduBus.content[0].line_direction
                    };
                    db.route_baidu.Add(rt_baidu);
                    db.SaveChanges();
                    //节点
                    string geostr = baiduBus.content[0].geo;
                    geostr = geostr.Substring(geostr.LastIndexOf("|") + 1);//去头部
                    geostr = geostr.Substring(0, geostr.Length - 1);//去尾部                    
                    string[] nodes = geostr.Split(new char[] { ',' });
                    for (int i = 0; i < nodes.Length; i = i + 2)
                    {
                        db.node_baidu.Add(new node_baidu
                        {
                            x = nodes[i],
                            y = nodes[i + 1],
                            routeid = rt_baidu.id
                            //note = baiduBus.content[0].geo
                        });
                    }
                    //站点表                    
                    var stations = baiduBus.content[0].stations;
                    foreach (var item in stations)
                    {
                        station_baidu station = new station_baidu();
                        station.routeid = rt_baidu.id;
                        //station.note = string.Join<BaiduBusFiddler.Station>(",", stations.ToArray());
                        station.name = item.name;
                        var strTemp = item.geo.Substring(item.geo.LastIndexOf("|") + 1);
                        strTemp = strTemp.Substring(0, strTemp.Length - 1);
                        string[] xy = strTemp.Split(new char[] { ',' });
                        station.x = xy[0];
                        station.y = xy[1];
                        db.station_baidu.Add(station);
                    }

                    db.SaveChanges();
                    #endregion
                }
                //天地图
                if (dataReceived.fullUrl.Contains("query"))
                {
                    #region 天地图
                    iSeesion.utilDecodeResponse();
                    string str = iSeesion.GetResponseBodyAsString();
                    if (str.Contains("linepoint"))
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                        TianDiTuBusFiddler.TianDiTuBus tianditubus = js.Deserialize<TianDiTuBusFiddler.TianDiTuBus>(str);
                        BaiduBusFiddler.BusEntities db = new BusEntities();
                        //线路
                        route_tianditu rt_tianditu = new route_tianditu
                        {
                            describe = tianditubus.linename,
                            name = tianditubus.linename,
                            line_direction = Helper.Substring(tianditubus.linename, "-", ")")
                        };
                        db.route_tianditu.Add(rt_tianditu);
                        db.SaveChanges();

                        //站点
                        var stations = tianditubus.station;
                        foreach (var item in stations)
                        {
                            station_tianditu station = new station_tianditu();
                            station.routeid = rt_tianditu.id;
                            station.name = item.name;
                            string[] xy = item.lonlat.Split(new char[] { ',' });
                            station.x = xy[0];
                            station.y = xy[1];
                            db.station_tianditu.Add(station);
                        }
                        //节点
                        string geostr = tianditubus.linepoint;
                        string[] nodes = geostr.Split(new char[] { ';' });
                        for (int i = 0; i < nodes.Length - 1; i++)
                        {
                            string[] xy = nodes[i].Split(new char[] { ',' });
                            var node = new node_tianditu
                            {
                                routeid = rt_tianditu.id,
                                x = xy[0],
                                y = xy[1]
                            };
                            db.node_tianditu.Add(node);
                        }
                        db.SaveChanges();
                    }
                    #endregion
                }
                //腾讯地图
                if (dataReceived.fullUrl.Contains("qt=dt"))
                {
                    iSeesion.utilDecodeResponse();
                    string str = iSeesion.GetResponseBodyAsString();
                    str = str.Substring(0,str.Length-1).Substring(str.IndexOf("(") + 1);
                    if (str.Contains("poi"))
                    {
                        System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                        QQBusFiddler.QQBus qqbus = js.Deserialize<QQBusFiddler.QQBus>(str);
                        BaiduBusFiddler.BusEntities db = new BusEntities();
                        //线路
                        route_tencent rt_qq = new route_tencent 
                        { 
                         describe=qqbus.detail.poi.from+"-"+qqbus.detail.poi.to,
                         name = qqbus.detail.poi.name,
                         line_direction=qqbus.detail.poi.to
                        };
                        db.route_tencent.Add(rt_qq);
                        db.SaveChanges();
                        //站点
                        var stations = qqbus.detail.poi.stations;
                        foreach (var item in stations)
                        {
                            station_tencent station = new station_tencent();
                            station.routeid = rt_qq.id;                               
                            station.name = item.name;
                            station.x = item.pointx;
                            station.y = item.pointy;
                            db.station_tencent.Add(station);
                        }
                        //节点
                        string geostr = qqbus.detail.poi.points;
                        string[] nodes = geostr.Split(new char[]{';'});
                        for (int i = 0; i < nodes.Length; i++)
                        {
                            string[] xy = nodes[i].Split(new char[]{','});
                            var node = new node_tencent
                            {
                                routeid=rt_qq.id,
                                x=xy[0],
                                y=xy[1]
                            };
                            db.node_tencent.Add(node);
                        }
                        db.SaveChanges();

                    }
                }

            }
        }
        static void Main(string[] args)
        {

            List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();

            Fiddler.FiddlerApplication.SetAppDisplayName("FiddlerKiwi");

            #region AttachEventListeners

            Fiddler.FiddlerApplication.OnNotification += delegate(object sender, NotificationEventArgs oNEA)
            {
                Console.WriteLine("**通知: " + oNEA.NotifyString);
            };
            Fiddler.FiddlerApplication.Log.OnLogString += delegate(object sender, LogEventArgs oLEA)
            {
                Console.WriteLine("**日志: " + oLEA.LogString);
            };


            Fiddler.FiddlerApplication.BeforeRequest += delegate(Fiddler.Session oS)
            {
                oS.bBufferResponse = false;
                Monitor.Enter(oAllSessions);//添加session时必须加排他锁
                oAllSessions.Add(oS);
                Monitor.Exit(oAllSessions);
                oS["X-AutoAuth"] = "(default)";
                oS.RequestHeaders["Accept-Encoding"] = "gzip, deflate";
                if ((oS.oRequest.pipeClient.LocalPort == iSecureEndpointPort) && (oS.hostname == sSecureEndpointHostname))
                {
                    oS.utilCreateResponseAndBypassServer();
                    oS.oResponse.headers.SetStatus(200, "Ok");
                    oS.oResponse["Content-Type"] = "text/html; charset=UTF-8";
                    oS.oResponse["Cache-Control"] = "private, max-age=0";
                    oS.utilSetResponseBody("<html><body>Request for httpS://" + sSecureEndpointHostname + ":" + iSecureEndpointPort.ToString() + " received. Your request was:<br /><plaintext>" + oS.oRequest.headers.ToString());
                }
            };
            //Kiwi：raw原生的，获得原生数据参数的事件。decompressed（解压缩）chunk（块），gracefully（优雅的地），invalid（无效的），EXACTLY（完全正确）,compatible（兼容的）,Decryption（解码）, E.g.例如，masquerading（伪装）
            Fiddler.FiddlerApplication.AfterSessionComplete += delegate(Fiddler.Session oS)
            {
                if (oS != null)
                {
                    QueueSessions.Enqueue(oS);
                }

            };
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            #endregion AttachEventListeners

            Fiddler.CONFIG.bHookAllConnections = true;
            Fiddler.CONFIG.IgnoreServerCertErrors = true;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;
            CreateAndTrustRoot();
            int iPort = 8877;//设置为0，程序自动选择可用端口
            writeThread.Start();
            Fiddler.FiddlerApplication.Startup(iPort, oFCSF);
            #region 日志系统
            FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", iPort);
            FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
            FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());
            #endregion

            Console.WriteLine("Hit CTRL+C to end session.");

            // We'll also create a HTTPS listener, useful for when FiddlerCore is masquerading（伪装） as a HTTPS server
            // instead of acting as a normal CERN-style proxy server.
            //oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);
            //if (null != oSecureEndpoint)
            //{
            //    FiddlerApplication.Log.LogFormat("Created secure endpoint listening on port {0}, using a HTTPS certificate for '{1}'", iSecureEndpointPort, sSecureEndpointHostname);
            //}

            bool bDone = false;
            do//使用的是do while
            {
                Console.WriteLine("\nEnter a command [C=Clear; L=List; G=Collect Garbage; R=read SAZ;\n\tS=Toggle Forgetful Streaming; T=Trust Root Certificate; Q=Quit]:");
                Console.Write(">");
                ConsoleKeyInfo cki = Console.ReadKey();
                Console.WriteLine();
                switch (Char.ToLower(cki.KeyChar))
                {
                    case 'c':
                        Monitor.Enter(oAllSessions);
                        oAllSessions.Clear();
                        Monitor.Exit(oAllSessions);
                        WriteCommandResponse("Clear...");
                        FiddlerApplication.Log.LogString("Cleared session list.");
                        break;

                    case 'd':
                        FiddlerApplication.Log.LogString("FiddlerApplication::Shutdown.");
                        FiddlerApplication.Shutdown();
                        break;

                    case 'l':
                        WriteSessionList(oAllSessions);//【Kiwi】
                        break;

                    case 'g':
                        Console.WriteLine("Working Set:\t" + Environment.WorkingSet.ToString("n0"));
                        Console.WriteLine("Begin GC...");
                        GC.Collect();
                        Console.WriteLine("GC Done.\nWorking Set:\t" + Environment.WorkingSet.ToString("n0"));
                        break;

                    case 'q':
                        bDone = true;
                        DoQuit();
                        break;

                    case 'r':
#if SAZ_SUPPORT
                        ReadSessions(oAllSessions);
#else
                        WriteCommandResponse("This demo was compiled without SAZ_SUPPORT defined");
#endif
                        break;

                    case 't':
                        try
                        {
                            WriteCommandResponse("Result: " + Fiddler.CertMaker.trustRootCert().ToString());
                        }
                        catch (Exception eX)
                        {
                            WriteCommandResponse("Failed: " + eX.ToString());
                        }
                        break;

                    // Forgetful streaming
                    case 's':
                        bool bForgetful = !FiddlerApplication.Prefs.GetBoolPref("fiddler.network.streaming.ForgetStreamedData", false);
                        FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.ForgetStreamedData", bForgetful);
                        Console.WriteLine(bForgetful ? "FiddlerCore will immediately dump streaming response data." : "FiddlerCore will keep a copy of streamed response data.");
                        break;

                }
            } while (!bDone);
        }
        private static bool CreateAndTrustRoot()
        {
            if (!Fiddler.CertMaker.rootCertExists())
            {
                var bCreatedRootCertificate = Fiddler.CertMaker.createRootCert();
                if (!bCreatedRootCertificate)
                {
                    return false;
                }
            }
            if (!Fiddler.CertMaker.rootCertIsTrusted())
            {
                var bTrustedRootCertificate = Fiddler.CertMaker.trustRootCert();
                if (!bTrustedRootCertificate)
                {
                    return false;
                }
            }
            return true;
        }
        private static void WriteSessionList(List<Fiddler.Session> oAllSessions)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Session list contains...");
            //放到一个try块中
            try
            {
                Monitor.Enter(oAllSessions);//Monitor Kiwi-?
                foreach (Session oS in oAllSessions)
                {
                    //MIME Type，资源的媒体类型
                    Console.Write(String.Format("{0} {1} {2}\n{3} {4}\n\n", oS.id, oS.oRequest.headers.HTTPMethod, Ellipsize(oS.fullUrl, 60), oS.responseCode, oS.oResponse.MIMEType));
                }
            }
            finally
            {
                Monitor.Exit(oAllSessions);
            }
            Console.WriteLine();
            Console.ForegroundColor = oldColor;
        }
        /// <summary>
        /// 超过长度时的显示方式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="iLen"></param>
        /// <returns></returns>
        private static string Ellipsize(string s, int iLen)
        {
            if (s.Length <= iLen) return s;
            return s.Substring(0, iLen - 3) + "...";
        }
        public static void WriteCommandResponse(string s)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(s);
            Console.ForegroundColor = oldColor;
        }

        #region 退出
        /// <summary>
        /// 退出程序
        /// </summary>
        public static void DoQuit()
        {
            WriteCommandResponse("Shutting down...");
            if (null != oSecureEndpoint) oSecureEndpoint.Dispose();
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
        }
        /// <summary>
        /// When the user hits CTRL+C, this event fires.  We use this to shut down and unregister our FiddlerCore.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            DoQuit();
        }
        #endregion
    }
}
