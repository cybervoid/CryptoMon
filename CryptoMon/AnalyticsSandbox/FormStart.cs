using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CryptoLib;
namespace AnalyticsSandbox
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private void loadCoinDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CryptoLib.Statics.Analyzer = new CryptoLib.Analysis.Analyzer();
            CryptoLib.Statics.Analyzer.InitializeQueues();
        }
    }
}
