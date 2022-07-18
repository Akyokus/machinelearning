using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using System.Data;


namespace machinelearning
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Dosyanın okunacağı dizin
            string filePath = @"C:\Users\mehme\myapp\dataset.xlsx";

            List<double> yas = new List<double>();
            List<double> gubre = new List<double>();
            List<double> boy = new List<double>();

            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);


            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            IExcelDataReader excelReader;
            
            int counter = 0;


            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);




            //Veriler okunmaya başlıyor.
            while (excelReader.Read())
            {
                counter++;

                //ilk satır başlık olduğu için 2.satırdan okumaya başlanıyor
                if (counter > 1)
                {

                    boy.Add(excelReader.GetDouble(0));
                    yas.Add(excelReader.GetDouble(1));
                    gubre.Add(excelReader.GetDouble(2));

                }
            }

            //Test verileri ayrılıyor
            var dataCount = yas.Count * 70 / 100;
            
            //Okuma bitiriliyor.
            excelReader.Close();
            //değerlerin ortalamaları bulunuyor
            double boyOrtalama = 0;
            double yasOrtalama = 0;
            double gubreOrtalama = 0;
            double boyToplam = 0;
            double yasToplam = 0;
            double gubreToplam = 0;
            double cov = 0;
            for (var i = 0;i<boy.Count;i++)
            {
                boyToplam += + boy[i];
                boyOrtalama = boyToplam / boy.Count;

                yasToplam += yas[i];
                yasOrtalama = yasToplam / yas.Count;

                gubreToplam += gubre[i];
                gubreOrtalama = gubreToplam / gubre.Count;

                

            }

            Console.WriteLine("Yaş ortalama: " + yasOrtalama.ToString("0.##"));
            Console.WriteLine("Gübre ortalama: " + gubreOrtalama.ToString("0.##"));

            double yasSapma = 0;
            double gubreSapma = 0;
            for (var i = 0; i < dataCount; i++)
            {
                //cov yas-gubre hesaplanacak
                cov += ((gubre[i] - gubreOrtalama) * (yas[i] - yasOrtalama));


                //standart sapma hesaplama (yaş)
                yasSapma += (yas[i] - yasOrtalama) * (yas[i] - yasOrtalama);

                //standart sapma hesaplama (gubre)
                gubreSapma += (gubre[i] - gubreOrtalama) * (gubre[i] - gubreOrtalama);
                
            }
            //kovaryans yazdırılıyor
            cov = cov / dataCount;
            Console.WriteLine("Kovaryans(yas-gubre): "+ cov.ToString("0.##"));

            yasSapma /= (dataCount - 1);
            gubreSapma /= (dataCount - 1);

            
            yasSapma = Math.Sqrt(yasSapma);
            gubreSapma = Math.Sqrt(gubreSapma);

            //standart sapmalar yazdırılıyor
            Console.WriteLine("Yas sapma: "+yasSapma.ToString("0.##"));
            Console.WriteLine("Gübre sapma: "+ gubreSapma.ToString("0.##"));

            //kolerasyon hesaplanıyor
            double koleras = cov / (yasSapma * gubreSapma);

            //kolerasyon yazdırılıyor
            Console.WriteLine("Kolerans (yas ve gubre): "+koleras.ToString("0.##"));

            double carpimToplam = 0;
            double yaskare = 0;
            //yas ve gubrenin kareler toplamı hesaplanıyor
            for(int i = 0; i < dataCount; i++)
            {
                carpimToplam += (yas[i]*gubre[i]);
                //kareler toplamı hesaplanıyor (yas)
                yaskare += Math.Pow(yas[i],2);
                
            }

               //b hesaplama
            double b = (carpimToplam-(yasToplam*gubreToplam)) / ((dataCount * yaskare) - Math.Pow(yasToplam,2));

            //a hesaplama
            double a = Math.Abs(gubre[1]) - Math.Abs(b*yas[1]);



            //Regresyon denklemi yazdırılıyor
            Console.WriteLine("Basit regresyon denklemi: y=" + a.ToString("0.##")+"+" +b.ToString("0.##") + "x");
            List<double> varyasyon = new List<double>();
            //test veri sonuçları yazdırılıyor
            for (var i = dataCount; i < boy.Count; i++)
            {
                varyasyon.Add(a + yas[i] * b);
                Console.WriteLine(yas[i] + " Yaş verisi ve " + gubre[i]+" gübre verisi girildiğinde boy değerimiz = "+(a+yas[i]*b).ToString("0.##") + " olmaktadır.");
            }

            double sse = 0;
            //test sse hesaplanıyor
            for(var i = 0; i < varyasyon.Count; i++)
            {
                sse += (varyasyon[i] - boy[i])* (varyasyon[i] - boy[i]);
            }
            Console.WriteLine("Test SSE = "+sse.ToString("0.##"));
            


            


            
        }
    }
}
