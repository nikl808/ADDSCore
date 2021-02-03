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