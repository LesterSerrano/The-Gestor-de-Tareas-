using GestordeTaras.EN;
using GestordeTareas.DAL;
using GestordeTareas.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.BL
{
    public class InvitacionProyectoBL
    {
        private readonly IInvitacionProyectoDAL _invitacionProyectoDAL;
        private readonly IProyectoUsuarioDAL _proyectoUsuarioDAL;

        // Constructor para inyección de dependencias
        public InvitacionProyectoBL(IInvitacionProyectoDAL invitacionProyectoDAL,
                                    IProyectoUsuarioDAL proyectoUsuarioDAL)
        {
            _invitacionProyectoDAL = invitacionProyectoDAL;
            _proyectoUsuarioDAL = proyectoUsuarioDAL;
        }

        // MÉTODO PARA CREAR UNA NUEVA INVITACIÓN
        public async Task<int> EnviarInvitacionAsync(InvitacionProyecto invitacion)
        {
            var usuariosUnidos = await _proyectoUsuarioDAL.ObtenerUsuariosUnidosAsync(invitacion.IdProyecto);
            if (usuariosUnidos.Any(u => u.NombreUsuario == invitacion.CorreoElectronico))
                return -1; // Usuario ya está unido al proyecto

            var invitacionPendiente = await VerificarInvitacionPendiente(invitacion.CorreoElectronico, invitacion.IdProyecto);
            if (invitacionPendiente != null)
                return -2; // Ya existe una invitación pendiente

            return await _invitacionProyectoDAL.CrearInvitacionAsync(invitacion);
        }

        // MÉTODO PARA ACEPTAR UNA INVITACIÓN
        public async Task<int> AceptarInvitacionAsync(string token, int idUsuario, string correoUsuario)
        {
            var invitacion = await _invitacionProyectoDAL.ObtenerPorTokenAsync(token);
            if (invitacion == null) return 0;

            if (invitacion.CorreoElectronico != correoUsuario) return -2;
            if (invitacion.Estado != "Pendiente") return -3;

            var usuariosUnidos = await _proyectoUsuarioDAL.ObtenerUsuariosUnidosAsync(invitacion.IdProyecto);
            if (usuariosUnidos.Any(u => u.Id == idUsuario)) return -1;

            await _proyectoUsuarioDAL.UnirUsuarioAProyectoAsync(invitacion.IdProyecto, idUsuario);

            invitacion.Estado = "Aceptada";
            invitacion.IdUsuario = idUsuario;
            return await _invitacionProyectoDAL.ActualizarInvitacionAsync(invitacion);
        }

        // MÉTODO PARA RECHAZAR UNA INVITACIÓN
        public async Task<int> RechazarInvitacionAsync(string token, int idUsuario, string correoUsuario)
        {
            var invitacion = await _invitacionProyectoDAL.ObtenerPorTokenAsync(token);
            if (invitacion == null) return 0;

            if (invitacion.CorreoElectronico != correoUsuario) return -2;
            if (invitacion.Estado != "Pendiente") return -3;

            invitacion.Estado = "Rechazada";
            invitacion.IdUsuario = idUsuario;
            return await _invitacionProyectoDAL.ActualizarInvitacionAsync(invitacion);
        }

        // MÉTODO PARA ELIMINAR UNA INVITACIÓN POR SU ID
        public async Task<bool> LimpiarInvitacionPorIdAsync(int id)
        {
            return await _invitacionProyectoDAL.EliminarInvitacionPorIdAsync(id);
        }

        // MÉTODO PARA OBTENER UNA INVITACIÓN POR TOKEN
        public async Task<InvitacionProyecto> ObtenerPorTokenAsync(string token)
        {
            return await _invitacionProyectoDAL.ObtenerPorTokenAsync(token);
        }

        // MÉTODO PARA OBTENER INVITACIONES POR ESTADO
        public async Task<List<InvitacionProyecto>> ObtenerInvitacionesPorEstadoAsync(int idProyecto, List<string> estados)
        {
            return await _invitacionProyectoDAL.ObtenerInvitacionesPorEstadoAsync(idProyecto, estados);
        }

        // MÉTODO PARA OBTENER INVITACIONES POR PROYECTO
        public async Task<List<InvitacionProyecto>> ObtenerInvitacionesPorProyectoAsync(int idProyecto)
        {
            return await _invitacionProyectoDAL.ObtenerInvitacionesPorProyectoAsync(idProyecto);
        }

        // MÉTODO PARA VERIFICAR SI YA EXISTE UNA INVITACIÓN PENDIENTE
        public async Task<InvitacionProyecto> VerificarInvitacionPendiente(string correoElectronico, int idProyecto)
        {
            return await _invitacionProyectoDAL.ObtenerInvitacionPendienteAsync(correoElectronico, idProyecto);
        }
    }
}
