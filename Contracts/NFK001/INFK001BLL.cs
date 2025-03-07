using NFK001.Models.NFK001;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFK001.Contracts.NFK001
{
    public interface INFK001BLL
    {
        Task<EnCodeProcess> Update(string dataPlano);
    }
}
