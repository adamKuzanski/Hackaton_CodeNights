using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvigilationApp.Models
{
    public class FrameStats
    {
        public int NbOfPeopleOnImage { get; set; }
        public int NbOfPeopleWithMask { get; set; }
        public int NbOfPeopleWithOutMask { get; set; }
        public int NbOfCars { get; set; }
        public int NbOfCyclers { get; set; }
        public int FrameNb { get; set; } // Frame number for which the analysys was created 

    }
}
