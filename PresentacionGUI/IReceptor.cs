using Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentacionGUI
{
   public interface IReceptor
    {
        void Recibir(Persona persona);
    }
}
