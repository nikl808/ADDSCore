using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ADDSCore.Model
{
    public class AutoConSysQuestList:INotifyPropertyChanged
    {
        private string name;
        private string controlObject;
        private string controlAnalog;
        private string controlStruct;
        private string network;
        private string software;
        private string document;

        public int Id { get; set; }
        public string Name 
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string ControlObject
        {
            get { return controlObject; }
            set
            {
                controlObject = value;
                OnPropertyChanged("ControlObject");
            }
        }
        public string ControlObjectAnalog
        {
            get { return controlAnalog; }
            set
            {
                controlAnalog = value;
                OnPropertyChanged("ControlObjectAnalog");
            }
        }
        public string ControlStruct 
        {
            get { return controlStruct; }
            set
            {
                controlStruct = value;
                OnPropertyChanged("ControlStruct");
            }
        }
        public string Network 
        {
            get { return network; }
            set
            {
                network = value;
                OnPropertyChanged("Network");
            }
        }
        public string Software 
        {
            get { return software; }
            set
            {
                software = value;
                OnPropertyChanged("Software");
            }
        }
        public string Document 
        {
            get { return document; }
            set
            {
                document = value;
                OnPropertyChanged("Document");
            }
        }
        public List<Hardware> ControlCab { get; set; } = new List<Hardware>();
        public List<Parameter> Param { get; set; } = new List<Parameter>();

        public AutoConSysQuestList() { }
        //copy constructor
        public AutoConSysQuestList(AutoConSysQuestList prevList)
        {
            Id = prevList.Id;
            Name = prevList.Name;
            ControlObject = prevList.ControlObject;
            ControlObjectAnalog = prevList.controlAnalog;
            ControlStruct = prevList.ControlStruct;
            Network = prevList.Network;
            Software = prevList.Software;
            Document = prevList.Document;
            ControlCab = prevList.ControlCab;
            Param = prevList.Param;
    }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

    }

    public class Hardware : INotifyPropertyChanged
    {
        private string cabinet;
        private string suppVoltage;
        private string controlVoltage;
        private string mainFreq;
        private string protect;
        private string climate;
        private string cabCompos;

        public int Id { get; set; }
          
        public string Cabinet 
        {
            get { return cabinet; }
            set
            {
                cabinet = value;
                OnPropertyChanged("Cabinet");
            }
        }
        public string SuppVoltage 
        {
            get { return suppVoltage; }
            set
            {
                suppVoltage = value;
                OnPropertyChanged("SuppVoltage");
            }
        }
        public string ControlVoltage 
        {
            get { return controlVoltage; }
            set
            {
                controlVoltage = value;
                OnPropertyChanged("ControlVoltage");
            }
        }
        public string MainFreq 
        {
            get { return mainFreq; }
            set
            {
                mainFreq = value;
                OnPropertyChanged("MainFreq");
            }
        }
        public string Protect 
        {
            get { return protect; }
            set
            {
                protect = value;
                OnPropertyChanged("Protect");
            }
        }
        public string Climate 
        {
            get { return climate; }
            set
            {
                climate = value;
                OnPropertyChanged("Climate");
            }
        }
        public string CabCompos 
        {
            get { return cabCompos; }
            set
            {
                cabCompos = value;
                OnPropertyChanged("CabCompos");
            }
        }
        public int AutoConSysQuestListId { get; set; } //external key
        public AutoConSysQuestList autoConSysQuestList { get; set; } //navigation property

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

    public class Parameter
    {
        private string controlUnit;
        private string controlParams;

        public int Id { get; set; }
        public string ControlUnit 
        {
            get { return controlUnit; }
            set
            {
                controlUnit = value;
                OnPropertyChanged("ControlUnit");
            }
        }
        public string ControlParams {
            get { return controlParams; }
            set
            {
                controlParams = value;
                OnPropertyChanged("ControlParams");
            }
        }
        public int AutoConSysQuestListId { get; set; } //external key
        public AutoConSysQuestList autoConSysQuestList { get; set; } //navigation property

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

    public class ACSListContext : DbContext
    {
        public DbSet<AutoConSysQuestList> ACSQuestionList { get; set; }
        public DbSet<Hardware> hardware { get; set; }
        public DbSet<Parameter> parameter { get; set; }
        public ACSListContext(DbContextOptions<ACSListContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}