using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public CarSimulator()
        {
            InitializeComponent();
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            int id = 0; //セマンティックリンクID
            int startNum = 0;//スタート地点NUM

            #region 仮想ログデータ（JST，車速，車間距離）を生成する処理
            //ECOLOGデータを取得する処理
            DataTable ECOLOGData = EcologDopplerDao.GetSelectedData();//仮引数は，トリップIDや時間を指定することで取得できる．要適当に変更←SELECT以下変更済
            //ソートされたリンク（線）を取得する処理
            List<LinkData> linkList = getLinkList(id, startNum);
            //仮想ログデータを生成するメソッド
            DataTable logData = generateSimulationLog(ECOLOGData, linkList);
            #endregion

            DataTable EcologSimulationTable = new DataTable();//DataTableUtil.GetEcolog…
            //ECOLOG計算をして，仮想ECOLOGデータを生成するメソッド
            for (int i = 0; i < logData.Rows.Count; i++)
            {
                DataRow ecologRow = HagimotoEcologCalculator.CalcEcologSimulation();//TODO: 引数の設定、返り値をシミュレーションのデータに
                EcologSimulationTable.Rows.Add(ecologRow);
            }

            #region 仮想ECOLOGデータをデータベースインサートする処理
            //仮想ECOLOGデータをデータベースインサートするメソッド
            EcologSimulationDao.Insert(EcologSimulationTable);
            #endregion
        }

        private DataTable generateSimulationLog(DataTable ECOLOGData, List<LinkData> linkList)//仮想のECOLOGデータを生成
        {

            for (int i = 0; i < ECOLOGData.Rows.Count; i++)
            {
                //TODO: DataTable simulationTable = DataTableUtil.GetEcologTable();//これを返り値

                double longitude = 0, latitude = 0;
                Tuple<int, double> linkComp = searchLinkComponent(linkList, latitude, longitude);

                


            //TODO: linklistとlinkCompをつかって，車間距離を取った位置のデータを作る処理．
            // var row = GenerateSimulationLog();    
            //simulation.Rows.Add(row);

            }



            return new DataTable();//TODO生成したシミュレーションログを返す
        }

        private Tuple<int,double> searchLinkComponent(List<LinkData> linkList, double latitude, double longitude)
        {
            double minDist = 255;


            int tempNum = 0;
            string tempLinkId = null;
            double offset = 0;
            //各リンクセグメントに対して
            for (int i = 0; i < linkList.Count; i++)
            {
                TwoDimensionalVector linkStartEdge = new TwoDimensionalVector(linkList[i].START_LAT, linkList[i].START_LONG);
                TwoDimensionalVector linkEndEdge = new TwoDimensionalVector(linkList[i].END_LAT, linkList[i].END_LONG);
                TwoDimensionalVector GPSPoint = new TwoDimensionalVector(latitude, longitude);

                //線分内の最近傍点を探す
                TwoDimensionalVector matchedPoint = TwoDimensionalVector.nearest(linkStartEdge, linkEndEdge, GPSPoint);

                //最近傍点との距離
                double tempDist = TwoDimensionalVector.distance(GPSPoint, matchedPoint);


                //リンク集合の中での距離最小を探す
                if (tempDist < minDist)
                {

                    minDist = tempDist;

                    tempNum = linkList[i].NUM;
                    tempLinkId = linkList[i].LINK_ID;

                    offset = HubenyDistanceCalculator.CalcHubenyFormula(linkList[i].START_LAT, linkList[i].START_LONG, latitude, longitude);
                }
            }

            return new Tuple<int, double>(tempNum, offset);
        }

        private List<LinkData> getLinkList(int id, int startNum)
        {
            

            DataTable LinkTable = LinkDataGetterForMM.LinkTableGetter2(id);
            DataRow[] LinkRows = LinkTable.Select(null, "NUM");
            DataRow[] StartLink = LinkTable.Select("NUM = " + startNum);
            List<LinkData> linkList = new List<LinkData>();

            linkList.Add(new LinkData(Convert.ToString(StartLink[0]["LINK_ID"]), Convert.ToInt32(StartLink[0]["NUM"]),
                Convert.ToDouble(StartLink[0]["START_LAT"]), Convert.ToDouble(StartLink[0]["START_LONG"]),
                Convert.ToDouble(StartLink[0]["END_LAT"]), Convert.ToDouble(StartLink[0]["END_LONG"]), Convert.ToDouble(StartLink[0]["DISTANCE"])));

            //スタート地点のリンクを初期値に設定

            Boolean flag = true;
            int j = 0;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < LinkRows.Length; i++)
                {
                    if (Convert.ToDouble(LinkRows[i]["START_LAT"]) == linkList[j].END_LAT && Convert.ToDouble(LinkRows[i]["START_LONG"]) == linkList[j].END_LONG
                        && (Convert.ToDouble(LinkRows[i]["END_LAT"]) != linkList[j].START_LAT || Convert.ToDouble(LinkRows[i]["END_LONG"]) != linkList[j].START_LONG))
                    {

                        linkList.Add(new LinkData(Convert.ToString(LinkRows[i]["LINK_ID"]), Convert.ToInt32(LinkRows[i]["NUM"]),
                        Convert.ToDouble(LinkRows[i]["START_LAT"]), Convert.ToDouble(LinkRows[i]["START_LONG"]),
                        Convert.ToDouble(LinkRows[i]["END_LAT"]), Convert.ToDouble(LinkRows[i]["END_LONG"]), Convert.ToDouble(LinkRows[i]["DISTANCE"])));
                        j++;
                        flag = true;
                        break;
                    }
                }

            }
            return linkList;
        }


        private class LinkData
        {
            public string LINK_ID { get; set; }
            public int NUM { get; set; }
            public double START_LAT { get; set; }
            public double START_LONG { get; set; }
            public double END_LAT { get; set; }
            public double END_LONG { get; set; }

            public double DISTANCE { get; set; }

            public LinkData(String LINK_ID, int NUM, double START_LAT, double START_LONG, double END_LAT, double END_LONG, double DISTANCE)
            {
                this.LINK_ID = LINK_ID;
                this.NUM = NUM;
                this.START_LAT = START_LAT;
                this.START_LONG = START_LONG;
                this.END_LAT = END_LAT;
                this.END_LONG = END_LONG;
                this.DISTANCE = DISTANCE;
            }
        }
    }
}
