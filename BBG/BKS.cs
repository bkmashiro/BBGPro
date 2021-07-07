using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BBG
{
    class BKS
    {
        string mapName = "测试图片";
        DateTime dateTime;
        int size_y = 0;
        BlockData[] blockDatas;
        int[] usage;
        byte[,] result;
        public void G(string path)
        {

            dateTime = DateTime.Now;

            try
            {
                //初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();
                //创建xml的根节点
                XmlElement rootElement = myXmlDoc.CreateElement("BKS");
                //将根节点加入到xml文件中（AppendChild）
                myXmlDoc.AppendChild(rootElement);

                //初始化第一层的第一个子节点
                XmlElement firstLevelElement1 = myXmlDoc.CreateElement("Overview");
                //填充第一层的第一个子节点的属性值（SetAttribute）
                //firstLevelElement1.SetAttribute("ID", "11111111");
                //firstLevelElement1.SetAttribute("Description", "Made in China");
                //将第一层的第一个子节点加入到根节点下
                rootElement.AppendChild(firstLevelElement1);

                XmlElement secondLevelElement11 = myXmlDoc.CreateElement("Name");
                secondLevelElement11.InnerText = mapName;
                firstLevelElement1.AppendChild(secondLevelElement11);
                XmlElement secondLevelElement12 = myXmlDoc.CreateElement("Time");
                secondLevelElement12.InnerText = dateTime.ToBinary().ToString();
                firstLevelElement1.AppendChild(secondLevelElement12);
                XmlElement secondLevelElement13 = myXmlDoc.CreateElement("X_Value");
                secondLevelElement13.InnerText = result.GetLength(0).ToString();
                firstLevelElement1.AppendChild(secondLevelElement13);

                XmlElement secondLevelElement14 = myXmlDoc.CreateElement("Y_Value");
                secondLevelElement14.InnerText = size_y.ToString();
                firstLevelElement1.AppendChild(secondLevelElement14);

                XmlElement secondLevelElement15 = myXmlDoc.CreateElement("Z_Value");
                secondLevelElement15.InnerText = result.GetLength(1).ToString();
                firstLevelElement1.AppendChild(secondLevelElement15);


                XmlElement secondLevelElement16 = myXmlDoc.CreateElement("BlockPalette");
                for (int i = 0; i < blockDatas.Length; i++)
                {
                    XmlElement tmp_thirdLevelElement = myXmlDoc.CreateElement("Block");
                    tmp_thirdLevelElement.SetAttribute("id",i.ToString());
                    tmp_thirdLevelElement.SetAttribute("name",blockDatas[i].name);
                    tmp_thirdLevelElement.SetAttribute("count",usage[i].ToString());
                    secondLevelElement16.AppendChild(tmp_thirdLevelElement);
                }
                firstLevelElement1.AppendChild(secondLevelElement16);

                XmlElement firstLevelElement2 = myXmlDoc.CreateElement("Data");

                StringBuilder sb = new StringBuilder();

                firstLevelElement2.SetAttribute("layers", result.GetLength(0).ToString());

                rootElement.AppendChild(firstLevelElement2);

                for (int i = 0; i < result.GetLength(0); i++)
                {
                    for (int j = 0; j < result.GetLength(1); j++)
                    {
                        sb.Append(result[i, j].ToString() + ',');
                    }
                    XmlElement tmp_secondLevelElement = myXmlDoc.CreateElement("layer");
                    tmp_secondLevelElement.InnerText = sb.ToString();
                    firstLevelElement2.AppendChild(tmp_secondLevelElement);
                    sb.Clear();
                }
                //将xml文件保存到指定的路径下
                myXmlDoc.Save(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Init(string s,int x,int y,int z, BlockData[] block, int[] _usage, byte[,] res)
        {
            this.mapName = s;
            this.size_y = y;
            this.blockDatas = block;
            this.usage = _usage;
            this.result = res;
        }
    }
}
