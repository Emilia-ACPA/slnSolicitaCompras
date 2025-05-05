using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("UnidadeMedida")]
    public class UnidadeMedida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Descricao { get; set; } = string.Empty;

        public UnidadeMedida() { }
    }
}
