//using GestordeTaras.EN;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace GestordeTareas.DAL
//{
//    public class CommentDAL
//    {
//        // Crear un nuevo comentario
//        public static async Task<int> CreateCommentAsync(Comment comment)
//        {
//            using var dbContext = new ContextoBD();

//            if (comment.FechaComentario == default)
//                comment.FechaComentario = DateTime.Now;

//            await dbContext.Comment.AddAsync(comment);
//            return await dbContext.SaveChangesAsync();
//        }

//        // Obtener comentarios por ID de proyecto
//        public static async Task<List<Comment>> ObtenerCommentPorProyectoAsync(int idProyecto)
//        {
//            using var dbContext = new ContextoBD();

//            return await dbContext.Comment
//                .Where(c => c.IdProyecto == idProyecto)
//                .Include(c => c.Usuario)
//                .OrderBy(c => c.FechaComentario)
//                .ToListAsync();
//        }

//        // Eliminar comentario por ID
//        public static async Task<int> EliminarCommentAsync(int idComment)
//        {
//            using var dbContext = new ContextoBD();

//            var comment = await dbContext.Comment.FirstOrDefaultAsync(c => c.Id == idComment);
//            if (comment == null) return 0;

//            dbContext.Comment.Remove(comment);
//            return await dbContext.SaveChangesAsync();
//        }

//        // Obtener comentario por ID
//        public static async Task<Comment> ObtenerComentarioPorIdAsync(int idComment)
//        {
//            using var dbContext = new ContextoBD();

//            return await dbContext.Comment
//                .Include(c => c.Usuario)
//                .FirstOrDefaultAsync(c => c.Id == idComment);
//        }
//    }
//}

