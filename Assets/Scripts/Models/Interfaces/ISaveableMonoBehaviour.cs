using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models.Interfaces
{
    public interface ISaveableMonoBehaviour
    {
        void Save();
        void Load();
        string GetUniqueId();
    }

}
