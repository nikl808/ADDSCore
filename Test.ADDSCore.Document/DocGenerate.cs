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
                new HwCabinet(){ Name = "���", SuppVoltage = "220�", OperatVoltage="24�", RatedFreq = "50��", ProtectLevel="IP61", Climate="���1", Composition = "������ �����1"},
                new HwCabinet(){ Name = "���", SuppVoltage = "220�", OperatVoltage="24�", RatedFreq = "50��", ProtectLevel="IP62", Climate="���2", Composition = "������ �����2"},
                new HwCabinet(){ Name = "���", SuppVoltage = "380�", OperatVoltage="24�", RatedFreq = "60��", ProtectLevel="IP63", Climate="���3", Composition = "������ �����3"},
                new HwCabinet(){ Name = "���(�)", SuppVoltage = "380�", OperatVoltage="24�", RatedFreq = "60��", ProtectLevel="IP64", Climate="���4", Composition = "������ �����4"}
            };

            List<ControlParameter> parameters = new List<ControlParameter>()
            {
                new ControlParameter(){ControlHwName = "��������", Parameter = "���������1\n���������2\n���������3\n���������4"},
                new ControlParameter(){ControlHwName = "���������", Parameter = "���������1\n���������2\n���������3\n���������4"},
                new ControlParameter(){ControlHwName = "������������", Parameter = "����������1\n����������2\n����������3\n����������4"},
            };

            testObj = new AutomaSysQuestnaire()
            {
                ListName = "��� �����",
                ObjName = "��� �������",
                ControlAnalog = "������ �������",
                ControlStruct = "��������� �������",
                Network = "ModbusTCP",
                Document = "�������� ����������",
                Extra = "�������������� ����������",
                Software = "���������� �����������",
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