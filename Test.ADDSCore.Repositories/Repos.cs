using NUnit.Framework;
using System.Collections.Generic;
using ADDSCore.DataProviders;
using ADDSCore.Models.Business;
using System.Threading.Tasks;
using System;

namespace Test.ADDSCore.Repositories
{
    public class Tests
    {
        

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task GetList()
        {
            using (AutomaQuestnaireRepo repos = new AutomaQuestnaireRepo())
            {
                var result = await repos.GetEntitiesListAsync();
                Assert.IsNotNull(result);
                foreach (var it in result)
                    Console.WriteLine($"List name:{ it.ListName }\nObjects:{it.ObjName}\n");
            }
            
        }

        [Test]
        public async Task AddEntity()
        {
            AutomaSysQuestnaire testObj;

            List<HwCabinet> cabinets = new List<HwCabinet>()
            {
                new HwCabinet(){ Name = "ШСА", SuppVoltage = "220В", OperatVoltage="24В", RatedFreq = "50Гц", ProtectLevel="IP61", Climate="Ухл1", Composition = "Состав шкафа1"},
                new HwCabinet(){ Name = "ШАУ", SuppVoltage = "220В", OperatVoltage="24В", RatedFreq = "50Гц", ProtectLevel="IP62", Climate="Ухл2", Composition = "Состав шкафа2"},
                new HwCabinet(){ Name = "ПМУ", SuppVoltage = "380В", OperatVoltage="24В", RatedFreq = "60Гц", ProtectLevel="IP63", Climate="Ухл3", Composition = "Состав шкафа3"},
                new HwCabinet(){ Name = "ПМУ(М)", SuppVoltage = "380В", OperatVoltage="24В", RatedFreq = "60Гц", ProtectLevel="IP64", Climate="Ухл4", Composition = "Состав шкафа4"}
            };

            List<ControlParameter> parameters = new List<ControlParameter>()
            {
                new ControlParameter(){ControlHwName = "Мельница", Parameter = "МПараметр1\nМПараметр2\nМПараметр3\nМПараметр4"},
                new ControlParameter(){ControlHwName = "Двигатель", Parameter = "ДПараметр1\nДПараметр2\nДПараметр3\nДПараметр4"},
                new ControlParameter(){ControlHwName = "Маслостанция", Parameter = "МАПараметр1\nМАПараметр2\nМАПараметр3\nМАПараметр4"},
            };

            testObj = new AutomaSysQuestnaire()
            {
                ListName = "Имя листа",
                ObjName = "Имя объекта",
                ControlAnalog = "Аналог объекта",
                ControlStruct = "Структура объекта",
                Network = "ModbusTCP",
                Document = "Комплект документов",
                Extra = "Дополнительная информация",
                Software = "Програмное обеспечение",
                Cabinet = cabinets,
                Parameter = parameters,
            };

            using (AutomaQuestnaireRepo repos = new AutomaQuestnaireRepo())
            {
                repos.Create(testObj);
                repos.Save();
                var result = await repos.GetEntitiesListAsync();
                Assert.IsNotNull(result);
                foreach (var it in result)
                    Console.WriteLine($"List name:{ it.ListName }\nObjects:{it.ObjName}\n");
            }
        }
    }
}