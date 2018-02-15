using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECOLOGCarSimulator.Daos;
using ECOLOGCarSimulator.Models;
using ECOLOGCarSimulator.Utils;
using ECOLOGCarSimulator.Calculators;
using System.Device.Location;
using ECOLOGCarSimulator.Calculators.CalculatorComponents;
using ECOLOGCarSimulator.Inserters.Components;

namespace ECOLOGCarSimulator
{
    public partial class CarSimulator : Form
    {
        /*public CarSimulator()
        {
            InitializeComponent();
        }*/

        private void InsertButton_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\Users\isobe\Desktop\";//TODO: ファイルパス・ファイル名を読み込む必要アリ
            string fileName = "20180215182054UnsentGPS.csv";//TODO: ファイルパス・ファイル名を読み込む必要アリ
            DataTable RawDataTable = getDataTableFromCSV(filePath, fileName);//.csvからのデータを生データ用のDataTableに格納

            DataTable EcologSimulationTable = new DataTable();//仮想の走行ログから生成した仮想ECOLOGデータを格納するDataTableを作成
            EcologSimulationTable = DataTableUtil.GetEcologTable();//↑で生成したDataTableのスキーマをECOLOGテーブルの形にする

            //ECOLOG計算をして，仮想ECOLOGデータを生成するメソッド
            for (int i = 0; i < RawDataTable.Rows.Count; i++)
            {
                DataRow ecologRow = HagimotoEcologCalculator.CalcEcologSimulation(DataRow tripRow,
                    InsertDatum datum, InsertConfig.GpsCorrection.Normal);//TODO: 仮想のGPSログを元にtripRow, Datumを設定
                EcologSimulationTable.Rows.Add(ecologRow);//仮想ECOLOGデータ格納用のDatatableにECOLOGデータを1行挿入
            }

            //仮想ECOLOGデータのDataTableをデータベースへインサート
            EcologSimulationDao.Insert(EcologSimulationTable);
        }

        //.csvからDataTable型のデータを作成するメソッド
        private DataTable getDataTableFromCSV(string filePath, string fileName)//引数は.csvのファイルパス, ファイル名
        {
            //.csvファイルのパスに接続
            string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                + filePath + ";Extended Properties=\"text;HDR=No;FMT=Delimited\"";
            System.Data.OleDb.OleDbConnection con =
                new System.Data.OleDb.OleDbConnection(conString);

            //.csvをDataAdapter da に読み込む
            string commText = "SELECT * FROM [" + fileName + "]";
            System.Data.OleDb.OleDbDataAdapter da =
                new System.Data.OleDb.OleDbDataAdapter(commText, con);

            //DataTable型のデータdtに.csvの内容を格納，dataTableを返す
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }
    }
}
