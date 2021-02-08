using NUnit.Framework;
using System.Collections.Generic;
using ADDSCore.Models.Business;
using System.Windows.Documents;

namespace Test.ADDSCore.Document
{
    public class Tests
    {
        private AutomaSysQuestnaire testObj;
        
        [SetUp]
        public void Setup()
        {
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
        }

        [Test]
        public void GenerateDocument()
        {
            AutomaSysDocTemplate doc = new AutomaSysDocTemplate(@"d:\test.docx");
            doc.CreatePackage(testObj);
        }
    }
}