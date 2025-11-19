using GestordeTaras.EN;
using GestordeTareas.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.DAL
{
    public class InvitacionProyectoDAL : IInvitacionProyectoDAL
    {
        private readonly ContextoBD _dbContext;

        public InvitacionProyectoDAL(ContextoBD dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CrearInvitacionAsync(InvitacionProyecto invitacion)
        {
            _dbContext.InvitacionProyecto.Add(invitacion);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<InvitacionProyecto> ObtenerPorTokenAsync(string token)
        {
            return await _dbContext.InvitacionProyecto
                .FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<int> ActualizarInvitacionAsync(InvitacionProyecto invitacion)
        {
            _dbContext.InvitacionProyecto.Update(invitacion);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<InvitacionProyecto>> ObtenerInvitacionesRechazadasAsync()
        {
            return await _dbContext.InvitacionProyecto
                .Where(i => i.Estado == "Rechazada" || i.FechaExpiracion < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<bool> EliminarInvitacionPorIdAsync(int id)
        {
            var invitacion = await _dbContext.InvitacionProyecto.FirstOrDefaultAsync(i => i.Id == id);
            if (invitacion != null)
            {
                _dbContext.InvitacionProyecto.Remove(invitacion);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<InvitacionProyecto>> ObtenerInvitacionesPorEstadoAsync(int idProyecto, List<string> estados)
        {
            return await _dbContext.InvitacionProyecto
                .Where(i => i.IdProyecto == idProyecto && estados.Contains(i.Estado))
                .ToListAsync();
        }

        public async Task<List<InvitacionProyecto>> ObtenerInvitacionesPorProyectoAsync(int idProyecto)
        {
            return await _dbContext.InvitacionProyecto
                .Where(i => i.IdProyecto == idProyecto)
                .ToListAsync();
        }

        public async Task<InvitacionProyecto> ObtenerInvitacionPendienteAsync(string correoElectronico, int idProyecto)
        {
            return await _dbContext.InvitacionProyecto
                .FirstOrDefaultAsync(i => i.CorreoElectronico == correoElectronico
                                       && i.IdProyecto == idProyecto
                                       && i.Estado == "Pendiente");
        }
<<<<<<< HEAD

=======
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb
    }
}
