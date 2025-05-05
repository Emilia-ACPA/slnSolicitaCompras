using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("Usuario")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }

        public Usuario()
        {
            NomeUsuario = string.Empty;
            Senha = string.Empty;
        }
    }
}
