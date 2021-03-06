﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace TankFlow
{
    public partial class TankFlow : Form
    {
        const int WM_COPYDATA = 0x004A;
        const int EVENT = 30000;
        const int BATTLE_START = 36278;
        const int BATTLE_END = 36279;
        const int SPOTED = 36281;
        const int UNSPOTED = 36280;
        const int DISABLE_PANEL = 36282;
        const int ENALBE_PANEL = 36283;
        const int STARTRECOGNIZE = 36284;
        const int STOPRECOGNIZE = 36285;


        private const string GameClass = "UnityWndClass";
        private const string GameName = "Tank Battle";
        private bool show_damage_panel = true;
        private List<Damage> damage_list = new List<Damage>();
        private Damage[] temp_damage = new Damage[6];

        private string user_id = "-1";

        public SocketClient python_client;

        private float rec_alpha = 0;
        private string rec_text = "正在识别...";
        public string tank_client;

        private int shoot_sum = 0;
        private int hit_sum = 0;
        private int penertrate_sum = 0;
        private int maxdamage_sum = 0;
        

        /// <summary>
        ///  造成的总伤害
        /// </summary>
        private int damage_sum = 0;

        /// <summary>
        ///  承受总伤害
        /// </summary>
        private int shield_sum = 0;

        public TankFlow()
        {
            InitializeComponent();
            initWindow();
            this.postimer.Stop();
            this.StartPosition = FormStartPosition.Manual;
            followPosition();
            this.user_id = "-1";
            SocketManager.StartServer(10824, new MessageReceiver(this), false);
        }

        public void HandleRecognizeResult(string result, int type)
        {
            if (type == 2)
            {
                SocketManager.SendData(this.tank_client, "1_" + result);
                MethodInvoker mi = new MethodInvoker(() =>
                {
                    this.recognizeTimer.Stop();
                    this.clearRec();
                    this.DrawScore();
                });
                this.BeginInvoke(mi);
            }
        }


        //初始化窗口数据
        private void initWindow()
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            label6.Text = "";
            for (int i = 0; i < 6; i++)
                temp_damage[i] = new Damage("");
            drawLabels();
            //DrawSpot("");
            DrawBulb(false);
            this.shoot_sum = 0;
            this.hit_sum = 0;
            this.penertrate_sum = 0;
            this.maxdamage_sum = 0;
            this.damage_sum = 0;
            this.shield_sum = 0;
            this.DrawScore();
        }

        //添加字符串至窗口
        public void PushString(Damage s)
        {
            for (int i = 5; i > 0; i--)
                temp_damage[i] = temp_damage[i - 1];
            temp_damage[0] = s;
            drawLabels();
        }

        /// <summary>
        /// 绘制伤害面板上的所有伤害数据
        /// </summary>
        /// <param name="m"></param>
        void drawLabels()
        {
            Point init = new Point(1, 80);
            int dy = 28;
            int width = 350, height = 20;
            Color green_color, red_color, border_color;
            border_color = Color.FromArgb(10, 0, 0);
            green_color = Color.FromArgb(80, 215, 120);
            red_color = Color.FromArgb(255,255,255);

            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);

            for (int i = 0; i < 6; i++)
            {
                Damage temp = temp_damage[i];
                if (!temp.valid)
                    continue;
                Rectangle rect = new Rectangle(init.X + 1, init.Y + i * dy, width, height);
                g.FillRectangle(back_brush, rect);
                

                Color left, right;
                if (temp_damage[i].friend)
                {
                    left = green_color;
                    right = red_color;
                }
                else
                {
                    left = red_color;
                    right = green_color;
                }
                GDIDraw.Paint_Text(temp.fillspace(10, temp.source), rect, left, border_color, g, 12f);
                rect.Offset(80, 0);
                GDIDraw.Paint_Text("->", rect, Color.FromArgb(255,255,255), border_color, g, 12f);
                rect.Offset(15, 0);
                GDIDraw.Paint_Text(temp.fillspace(10, temp.victim), rect, right, border_color, g, 12f);
                rect.Offset(100, 0);
                GDIDraw.Paint_Text(temp.GetDamageType()+"("+temp.GetHitPartName()+")", rect, left, border_color, g, 12f);

            }
            g.Dispose();
            back_brush.Dispose();
        }

        private void DrawScore()
        {
            string line1 = "中 " + this.hit_sum.ToString();
            line1 = line1 + "      穿 " + this.penertrate_sum.ToString();
            line1 = line1 + "      暴 " + this.maxdamage_sum.ToString();
            string line2 = "伤 " + this.damage_sum.ToString();
            line2 = line2 + "      防 " + this.shield_sum.ToString();
            Point init = new Point(80,17);
            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);
            Rectangle rect1 = new Rectangle(init.X,init.Y,220,25);
            Rectangle rect2 = new Rectangle(init.X, init.Y + 25, 220, 30);
            g.FillRectangle(back_brush, rect1);
            g.FillRectangle(back_brush, rect2);
            Color font_color, border_color;
            border_color = Color.FromArgb(10, 0, 0);
            font_color = Color.FromArgb(50,190,243);
           
            GDIDraw.Paint_Text(line1, rect1, font_color, border_color, g, 11f);
            GDIDraw.Paint_Text(line2, rect2, font_color, border_color, g, 11f);
            g.Dispose();
            back_brush.Dispose();
        }

        private void DrawSpot(string text)
        {
            Point init = new Point(1, 4);
            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);
            Rectangle rect = new Rectangle(init.X, init.Y, 100, 30);
            g.FillRectangle(back_brush, rect);
            Color font_color, border_color;
            border_color = Color.FromArgb(10, 0, 0);
            font_color = Color.FromArgb(239, 32, 0);
            GDIDraw.Paint_Text(text, rect, font_color, border_color, g, 20f);
            g.Dispose();
            back_brush.Dispose();
        }

        private void DrawBulb(bool draw)
        {
            Point init = new Point(0, 0);
            Rectangle rect = new Rectangle(init.X, init.Y, 70, 70);
            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);
            g.FillRectangle(back_brush, rect);
            if (draw)
            {
                Image img = Image.FromFile("Resource//pao.png");
                GraphicsUnit units = GraphicsUnit.Pixel;                //单位设置成像素
                g.DrawImage(img, 0, 0, rect, units);
            }
            back_brush.Dispose();
            g.Dispose();
        }

        private void drawRec(int alpha)
        {
            Rectangle rect = new Rectangle(95, 0, 36, 39);
            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);
            g.FillRectangle(back_brush, rect);

            Color border_color = Color.FromArgb(10, 0, 0);
            Color font_color = Color.FromArgb(alpha, 80, 215, 120);
            GDIDraw.Paint_Text("●", rect, font_color, border_color, g, 25f);

            g.Dispose();
            back_brush.Dispose();
            this.drawRecText();

        }

        public void drawRecText()
        {
            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);
            Color border_color = Color.FromArgb(10, 0, 0);
            Rectangle rect2 = new Rectangle(120, 10, 220, 19);
            g.FillRectangle(back_brush, rect2);
            Color font_color2 = Color.FromArgb(80, 215, 120);
            GDIDraw.Paint_Text(this.rec_text, rect2, font_color2, border_color, g, 14f);
            g.Dispose();
            back_brush.Dispose();
        }

        public void clearRec()
        {
            Graphics g = this.CreateGraphics();
            Brush back_brush = new SolidBrush(this.BackColor);
            Rectangle rect2 = new Rectangle(95, 0, 250, 40);
            g.FillRectangle(back_brush, rect2);
            g.Dispose();
            back_brush.Dispose();
        }

        //消息处理函数
        /**/
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            /*
            is_lock = true;
            if (m.Msg == 32770)
                is_lock = false;
            if (is_lock)
            {
                base.DefWndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case WM_COPYDATA:
                    string s1 = getCopyMessage(ref m);
                    Log.AddLog("收到字符串:" + s1);
                    ReceivedData data = new ReceivedData(s1);
                    //this.HandleCopyData(data);

                    break;
                case EVENT:
                    OnHandleEvent((int)m.LParam);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
            */
            base.DefWndProc(ref m);
        }

        public void HandleCopyData(string host, ReceivedData data)
        {
            if (data.dataKey != 3)
            {
                this.tank_client = host;
            }
            MethodInvoker mi = new MethodInvoker(() =>
            {
                switch (data.dataKey)
                {
                    case 1://坦克造成伤害
                        Damage dama = new Damage(data.message);
                        //Log.AddLog(dama.Output());
                        if (dama.valid)
                        {
                            damage_list.Add(dama);
                            if (this.show_damage_panel)
                                PushString(dama);
                        }
                        break;
                    case 2://战斗结果
                        if (this.user_id != "-1")
                        {
                            BattleResult result = new BattleResult(this.user_id.ToString() + "," + data.message);
                            if (result.valid)
                            {
                                Thread th = new Thread(this.uploadBattleResult);
                                th.Start(result);
                            }
                        }
                        else
                        {
                            Log.AddLog("user_id=-1的时候");
                        }
                        break;
                    case 3://设置user_id
                       this.user_id = data.message;
                       Log.AddLog("设置ID成功 " + this.user_id);
                        break;
                    case 4://事件信息
                        int flag = int.Parse(data.message);
                        this.OnHandleEvent(flag);
                        break;
                    case 5://玩家击中目标
                        //Log.AddLog("玩家击中了目标:" + data.message);
                        this.hit_sum++;
                        string[] str = data.message.Split(',');
                        if (str[0]!= "5")
                        {
                            this.penertrate_sum++;
                            this.damage_sum += int.Parse(str[1]);
                            if (str[0] == "9")
                                this.maxdamage_sum++;
                        }
                        this.DrawScore();
                        break;
                    case 6://玩家被击中
                        this.shield_sum += int.Parse(data.message);
                        this.DrawScore();
                        break;
                }
            });
            this.BeginInvoke(mi);


        }

        private void uploadBattleResult(object data)
        {
            BattleResult result = (BattleResult)data;
            Log.AddLog("开始上传战斗结果");
            bool up_result = HttpConnect.UploadBattleResult(result);
            if (up_result) Log.AddLog("上传战斗结果成功 " + result.tank);
            else Log.AddLog("上传战斗结果失败 " + result.tank);
        }

        private void uploadData()
        {
            List<Damage> valid_damage = new List<Damage>();
            foreach (Damage data in damage_list)
            {
                if (data.valid && data.grade > 5)
                {
                    valid_damage.Add(data);
                }
            }
            damage_list.Clear();
            Thread th = new Thread(()=>
            {
                UploadManager manager = new UploadManager(valid_damage);
                manager.Upload();//开始上传数据
            });
            th.Start();
        }

        private void RequestAdvanceFeature()
        {
            Thread th = new Thread(() =>
              {
                  string advance = "0";
                  if (this.user_id != "-1")
                  {
                      advance = HttpConnect.BattleStart(this.user_id.ToString());
                  }
                  if (advance == "0")
                  {
                      Log.AddLog("未验证高级功能权利");
                  }
                  else
                  {
                      Log.AddLog("已验证高级功能权利"+advance);
                  }
                  SocketManager.SendData(this.tank_client, "3_" + advance);
              });
            th.Start();
          
        }

        private void StopAdvanceFeature()
        {
            bool advance = false;
            if (this.user_id != "-1")
            {
                advance = HttpConnect.BattleEnd(this.user_id.ToString());
            }
            if (advance)
            {
                Log.AddLog("已停止高级功能权利");
            }
        }

        private void OnHandleEvent(int flag)
        {
            switch (flag)
            {
                case BATTLE_START:
                    initWindow();
                    this.Show();
                    this.postimer.Start();
                    Log.AddLog("战斗开始");
                    PrepareSoundRecognize();
                    break;
                case BATTLE_END:
                    Log.AddLog("战斗结束");
                    this.postimer.Stop();
                    this.recognizeTimer.Stop();
                    this.clearRec();
                    initWindow();
                    this.Hide();
                    StopAdvanceFeature();
                    uploadData();
                    new Thread(() =>
                    {
                        if (python_client != null&&python_client.IsConnect())
                        {
                            python_client.SendData("1 stop_recognize");
                            this.python_client.DisConnect();
                        }
                    }).Start();
                    break;
                case SPOTED:
                    //this.spot_state.Visible = true;
                    Log.AddLog("已被点亮");
                    DrawBulb(true);
                    this.Show();
                    this.postimer.Start();
                    break;
                case UNSPOTED:
                    //this.spot_state.Visible = false;
                    Log.AddLog("已经灭点");
                    DrawBulb(false);
                    break;
                case DISABLE_PANEL:
                    Log.AddLog("战斗面板已禁用");
                    this.show_damage_panel = false;
                    break;
                case ENALBE_PANEL:
                    Log.AddLog("战斗面板已启用");
                    this.show_damage_panel = true;
                    break;
                case STARTRECOGNIZE:
                    Log.AddLog("开始语音识别...");
                    this.StartSoundRecognize();
                    break;
                case STOPRECOGNIZE:
                    Log.AddLog("结束语音识别...");
                    this.StopSoundRecognize();
                    break;
                case 36286://玩家射击了一次
                    Log.AddLog("玩家射击了一次");
                    this.shoot_sum++;
                    this.DrawScore();
                    break;
                case 36287:
                    Log.AddLog("请求刷新高级特性");
                    RequestAdvanceFeature();
                    break;
                case 36288:
                    Log.AddLog("玩家被击毁");
                    DrawBulb(false);
                    break;
                default:
                    Log.AddLog("未知消息类型：" + flag);
                    break;
            }
        }

        

        private void PrepareSoundRecognize()
        {
            new Thread(() =>
            {
                this.python_client = new PythonClient(34567, "127.0.0.1", this);
                this.python_client.ConnectServer();
            }).Start();
        }

        private void StopSoundRecognize()
        {
            if (python_client != null)
            {
                python_client.SendData("1 stop_recognize");
            }
            this.recognizeTimer.Stop();
            this.clearRec();
            this.DrawScore();
        }

        private void StartSoundRecognize()
        {
            this.rec_text = "正在识别...";
            this.recognizeTimer.Start();
            new Thread(() =>
            {
                if (this.python_client == null)
                {
                    this.python_client = new PythonClient(34567, "127.0.0.1", this);
                    this.python_client.ConnectServer();
                }
                python_client.SendData("1 start_recognize");

            }).Start();
           
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        //获取传递过来的字符串
        private string getCopyMessage(ref System.Windows.Forms.Message m)
        {
            COPYDATASTRUCT mystr = new COPYDATASTRUCT();
            Type mytype = mystr.GetType();
            mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
            int flag = (int)mystr.dwData;
            byte[] bt = new byte[mystr.cbData];
            Marshal.Copy(mystr.lpData, bt, 0, bt.Length);
            string m2 = System.Text.Encoding.Unicode.GetString(bt);
            return m2;
        }

        //定时器执行动作
        private void Postimer_Tick(object sender, EventArgs e)
        {
            followPosition();
        }

        //设置窗口跟随游戏窗口移动
        private void followPosition()
        {
            IntPtr hwnd = BaseAPI.FindWindow(GameClass, GameName);
            IntPtr forgeWindow = BaseAPI.GetForegroundWindow();
            if (hwnd != forgeWindow)
            {
                this.Location = new System.Drawing.Point(-300, -300);
                return;
            }
            this.TopMost = true;
            BaseAPI.RECT client;
            BaseAPI.GetWindowRect(hwnd, out client);
            this.Location = new System.Drawing.Point((int)(client.Left + 165), (int)(client.Bottom - 220));
        }

        private void TankFlow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void RecognizeTimer_Tick(object sender, EventArgs e)
        {
            int alp;
            if (this.rec_alpha > 3.1415f)
            {
                this.rec_alpha -= 3.1415f;
            }
            alp = (int)(255 * Math.Sin(this.rec_alpha));
            this.drawRec(alp);
            this.rec_alpha += 0.3f;
        }
    }
}
