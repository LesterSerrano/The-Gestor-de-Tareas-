using GestordeTaras.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.DAL.Interfaces
{
    public interface IInvitacionProyectoDAL
    {
        Task<int> CrearInvitacionAsync(InvitacionProyecto invitacion);
        Task<InvitacionProyecto> ObtenerPorTokenAsync(string token);
        Task<int> ActualizarInvitacionAsync(InvitacionProyecto invitacion);
        Task<List<InvitacionProyecto>> ObtenerInvitacionesRechazadasAsync();
        Task<bool> EliminarInvitacionPorIdAsync(int id);
        Task<List<InvitacionProyecto>> ObtenerInvitacionesPorEstadoAsync(int idProyecto, List<string> estados);
        Task<List<InvitacionProyecto>> ObtenerInvitacionesPorProyectoAsync(int idProyecto);
        Task<InvitacionProyecto> ObtenerInvitacionPendienteAsync(string correoElectronico, int idProyecto);
    }
}
