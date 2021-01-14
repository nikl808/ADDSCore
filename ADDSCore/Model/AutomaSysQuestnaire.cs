using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADDSCore.Model
{
    public class AutomaSysQuestnaire
    {
        public int Id { get; set; }
        public string ListName { get; set; }
        public string ObjName { get; set; }
        public string ControlAnalog { get; set; }
        public string ControlStruct { get; set; }
        public string Network { get; set; }
        public string Software { get; set; }
        public string Document { get; set; }
        public string Extra { get; set; }
        public List<HwCabinet> Cabinet { get; set; } = new List<HwCabinet>();
        public List<ControlParameter> Parameter { get; set; } = new List<ControlParameter>();
        public List<AutomaSysQuestnaire> list { get; set; } = new List<AutomaSysQuestnaire>();

        public AutomaSysQuestnaire() { }

        public AutomaSysQuestnaire(AutomaSysQuestnaire prevObj)
        {
            Id = prevObj.Id;
            ListName = prevObj.ListName;
            ObjName = prevObj.ObjName;
            ControlAnalog = prevObj.ControlAnalog;
            ControlStruct = prevObj.ControlStruct;
            Network = prevObj.Network;
            Software = prevObj.Software;
            Document = prevObj.Document;
            Extra = prevObj.Extra;
            Cabinet = prevObj.Cabinet;
            Parameter = prevObj.Parameter;
            list = prevObj.list;
        }
    }
    [Owned]
    public class HwCabinet
    {
        public string Name { get; set; }
        public string SuppVoltage { get; set; }
        public string OperatVoltage { get; set; }
        public string RatedFreq { get; set; }
        public string ProtectLevel { get; set; }
        public string Climate { get; set; }
        public string Composition { get; set; }
    }
    [Owned]
    public class ControlParameter 
    {
        public string ControlHwName { get; set; }
        public string Parameter { get; set; }
    }

    public class AutomaSysContext : DbContext
    {
        public DbSet<AutomaSysQuestnaire> AutomaQuestnaire { get; set; }
        public AutomaSysContext(DbContextOptions<AutomaSysContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}


