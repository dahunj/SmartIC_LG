using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartICAVI
{
    public partial class TsMesForm : Form
    {
        public AxaxTibcoRv.AxucTibcoRv Tibco { get { return this.axucTibcoRv1; } }

        public string ServiceName { get; set; }
        public string NetworkName { get; set; }
        public string DaemonName { get; set; }

        public string PubSubject { get; set; }
        public int PubTimeOut { get; set; }
        public int PubRvType { get; set; }

        public string Password { get; set; }

        public string SubSubject { get; set; }
        public int SubRvType { get; set; }

        public TsMesForm()
        {
            InitializeComponent();

            ServiceName = "3300";
            NetworkName = ";239.100.100.2";
            DaemonName = "156.147.113.133:3300";

            PubSubject = "TS3.REQ.SPCSRV";
            PubTimeOut = 30;
            PubRvType = 2;

            Password = "";

            SubSubject = "TS3.ECS";
            SubRvType = 0;
        }

        private void TsMesForm_Load(object sender, EventArgs e)
        {
            SetParams();
            TibcoOpen();
        }

        private void TsMesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TibcoClose();
        }

        public void SetParams()
        {
            this.axucTibcoRv1.ServiceName = ServiceName;
            this.axucTibcoRv1.NetworkName = NetworkName;
            this.axucTibcoRv1.DaemonName = DaemonName;

            this.axucTibcoRv1.PubSubject = PubSubject;
            this.axucTibcoRv1.PubTimeOut = PubTimeOut;
            this.axucTibcoRv1.PubRvType = (axTibcoRv.ePubRvType)PubRvType;

            this.axucTibcoRv1.Password = Password;

            this.axucTibcoRv1.SubSubject = SubSubject;
            this.axucTibcoRv1.SubRvType = (axTibcoRv.eSubRvType)SubRvType;
        }

        public bool TibcoOpen()
        {
            return this.axucTibcoRv1.TibRvOpen();
        }

        public void TibcoClose()
        {
            this.axucTibcoRv1.TibRvClose();
        }

    }
}
