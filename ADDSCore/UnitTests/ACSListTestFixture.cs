using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using ADDSCore.Model;
using NUnit.Framework;

namespace ADDSCore.UnitTests
{
    class ACSListTestFixture
    {
        private ACSListContext db;

        [SetUp]
        public void Initialize_Test()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ACSListContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            db = new ACSListContext(options);
        }

        [Test]
        public void CreateEntries()
        {
            AutoConSysQuestList list = new AutoConSysQuestList()
            {
                Name = "Асу мельницей",
                ControlObject = "Мельница МШЦ-3750×5850",
                ControlObjectAnalog = "аналогична локальной АСУ мельницей МШЦ-3750×5850",
                ControlStruct = "Функции верхнего уровня выполняет АСУТП. Для локальной АСУ мельницей верхний уровень не является обязательным. Все функции по управлению мельницей, гидрооборудованием, противоаварийную защиту, отображение и регистрацию параметров и т.п. АСУ мельницы выполняет самостоятельно, независимо от АСУТП.",
                Network = "ModbusTCP",
                Software = "Программирование контроллера ШАУ ведется на языках МЭК. Возможна разработка прикладного ПО в соответствии концепции разработки ПО Заказчика при предоставлении описании этой концепции.",
                Document = "На все комплектно поставляемое оборудование предоставляется полный комплект сопроводительной документации (паспорта, руководства по эксплуатации и т.п.). В составе сопроводительной документации присутствуют все необходимые для эксплуатации математические и программные документы.",
            };


            Hardware hw1 = new Hardware()
            {
                autoConSysQuestList = list,
                Cabinet = "ШСА",
                SuppVoltage = "380 В",
                ControlVoltage = "24 В(DC)",
                MainFreq = "50 Hz",
                Protect = "не менее IP31",
                Climate = "УХЛ4",
                CabCompos = "Шкаф силовой аппаратуры (ШСА). Обеспечивает размещение пускорегулирующей аппаратуры двигателями и нагревателями на основе устройств типа Simocode pro (тип S или V). Устройства Simocode оснащены панелями оператора по типу 3UF7200 (Simocode pro S) или 3UF7210 (Simocode pro V). Панели необходимо вынесены на дверь ШСА. При мощности электродвигателя вспомогательного привода более 37кВт (уточняется при проектировании) будет установлено устройство плавного пуска фирмы ABB (серия PSTX). На вводе ШСА установлен выкатной (втычной) вводной автоматический выключатель с широкими пределами регулирования уставок (или выключатель нагрузки), многофункциональный измерительные прибор PAC3200 с коммуникацией и ограничители перенапряжений картриджного типа (класс II). PAC можно использовать в качестве прибора технического учёта электроэнергии."
            };

            Hardware hw2 = new Hardware()
            {
                autoConSysQuestList = list,
                Cabinet = "ШАУ",
                SuppVoltage = "220 В",
                ControlVoltage = "24 В(DC)",
                MainFreq = "50 Hz",
                Protect = "IP65",
                Climate = "УХЛ4",
                CabCompos = "Шкаф автоматики и управления (ШАУ) мельницей. Предназначен для реализации алгоритмов управления и СБиПАЗ. В ШАУ находится контроллер Simatic S7-1500 (прикладное ПО - Tia Portal STEP7 v13/v14), который связан со станциями удаленного в вода вывода и с АСУТП. Контроллер имеет возможность самодиагностики. ШАУ может управлять мельницей независимо от АСУТП. Через управляемый коммутатор (тип комутатора по согласованию с Заказчиком) по оптическому каналу ШАУ передает в АСУТП все данные о состоянии оборудования и получает от АСУТП управляющие команды и величины уставок (в случае необходимости)."
            };
            
            Parameter param1 = new Parameter()
            {
                autoConSysQuestList = list,
                ControlUnit = "Мельница",
                ControlParams = "Счетчик учета мото - часов работы с начала эксплуатации Предпусковой светозвуковой сигнал Светозвуковой сигнал аварийного останова"
            };

            Parameter param2 = new Parameter()
            {
                autoConSysQuestList = list,
                ControlUnit = "Электродвигатель главного привода",
                ControlParams = "Контроль температуры подшипников Контроль параметров системы охлаждения (кроме двигателей с воздушным охлаждением и частотного преобразователя) Контроль температуры обмоток"
            };

            
            db.ACSQuestionList.Add(list);
            db.hardware.AddRange(hw1, hw2);
            db.parameter.AddRange(param1, param2);
            db.SaveChanges();
        }

        [Test]
        public void ShowAllLists()
        {
            var list = db.ACSQuestionList.ToList();
            using(StreamWriter stream = new StreamWriter("dblog.txt",true))
            {
                foreach (AutoConSysQuestList lst in list)
                {
                    stream.WriteLine($"{lst.Id}.{lst.Name}:");
                    stream.WriteLine($"{lst.ControlObject}");
                    stream.WriteLine($"{lst.ControlObjectAnalog}");
                    stream.WriteLine($"{lst.ControlStruct}");
                    stream.WriteLine($"{lst.Network}");
                    stream.WriteLine($"{lst.Software}");
                    stream.WriteLine($"{lst.Document}");

                    foreach (Hardware hw in lst.ControlCab)
                    {
                        stream.WriteLine($"{hw.Id}.{hw.Cabinet}:");
                        stream.WriteLine($"{hw.SuppVoltage}");
                        stream.WriteLine($"{hw.ControlVoltage}");
                        stream.WriteLine($"{hw.MainFreq}");
                        stream.WriteLine($"{hw.Protect}");
                        stream.WriteLine($"{hw.Climate}");
                        stream.WriteLine($"{hw.CabCompos}");
                    }

                    foreach (Parameter param in lst.Param)
                        stream.WriteLine($"{param.Id}.{param.ControlUnit} : {param.ControlParams}");
                }
            }
        }

        [TearDown]
        public void TeardownTest()
        {
            db.Dispose();
        }
    }
}
