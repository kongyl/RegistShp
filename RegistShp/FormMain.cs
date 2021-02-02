using OSGeo.OGR;
using OSGeo.OSR;
using RegistShp.Dao;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RegistShp
{
    public partial class FormMain : Form
    {
        private string shpPath;

        public FormMain()
        {
            InitializeComponent();

            Ogr.RegisterAll();
        }

        private void buttonIn_Click(object sender, EventArgs e)
        {
            shpPath = textBoxIn.Text = "";
            if (openFileDialogIn.ShowDialog() == DialogResult.OK)
            {
                shpPath = textBoxIn.Text = openFileDialogIn.FileName;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (shpPath == null || shpPath.Trim().Equals(""))
            {
                MessageBox.Show("请选择Shapefile");
                return;
            }
            buttonOk.Enabled = false;

            backgroundWorkerMain.RunWorkerAsync();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void backgroundWorkerMain_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            backgroundWorkerMain.ReportProgress(0, "读取空间参考");
            DataSource ds = Ogr.Open(shpPath, 0);
            Layer layer = ds.GetLayerByIndex(0);
            SpatialReference sr = layer.GetSpatialRef();
            if (!sr.GetAttrValue("AUTHORITY", 1).Equals("4326"))
            {
                MessageBox.Show("目前仅支持WGS84空间参考");
                return;
            }

            string sqlPath = shpPath.Substring(0, shpPath.Length - 4);

            backgroundWorkerMain.ReportProgress(1, "正在注册矢量");

            CountryDao.Drop();
            if (CountryDao.UpdateVirtualShp(sqlPath) == -1)
            {
                MessageBox.Show("矢量注册失败");
                return;
            }
            List<long> codeList = CountryDao.FindAllCodeNew();
            if (codeList == null || codeList.Count == 0)
            {
                MessageBox.Show("矢量没有正确的code_new字段信息");
                return;
            }

            MessageBox.Show("矢量注册完成");
        }

        private void backgroundWorkerMain_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            toolStripStatusLabelMain.Text = e.UserState.ToString();
        }

        private void backgroundWorkerMain_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabelMain.Text = "就绪";
            buttonOk.Enabled = true;
        }
    }
}
