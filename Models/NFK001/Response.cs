using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFK001.Models.NFK001
{
    public class Response
    {
        public DateTime DATAPLANO { get; set; }
        public decimal CODFILEMP { get; set; }
        public decimal CODMER { get; set; }
        public decimal NUMSEQLSTPCOFRN { get; set; }
        public decimal CODSTAAPV { get; set; }
        public decimal CODFRN { get; set; }
        public decimal CODCID { get; set; }
        public string? CODESTUNI { get; set; }
        public decimal VLRPCOLSTPCO { get; set; }
        public decimal PERIPILSTPCO { get; set; }
        public decimal PERICMITE { get; set; }
        public string? TIPFRT { get; set; }
        public DateTime DATHRAAPVLSTPCOFRN { get; set; }
        public decimal PERALQICMNOTFSC { get; set; }
        public decimal INDORIMERFRN { get; set; }
        public decimal PERTBTICMFNDPBR { get; set; }
    }
}