using Microsoft.EntityFrameworkCore;
using AbrigueSe.Models;
using System.Linq; 
using System.Threading.Tasks; 

namespace AbrigueSe.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DbSets (ensure all are present)
        public DbSet<Pais> Pais { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }        
        public DbSet<TipoUsuario> TipoUsuario { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Abrigo> Abrigo { get; set; }
        public DbSet<CheckIn> CheckIn { get; set; } 
        public DbSet<Recurso> Recurso { get; set; }
        public DbSet<EstoqueRecurso> EstoqueRecurso { get; set; }

        public async Task<int> GetNextSequenceValueAsync(string sequenceName)
        {
            // Ensure sequence name is just the name, not schema.name if your DB user has default schema access
            // or if the sequence is public/accessible.
            // For Oracle, sequenceName.NEXTVAL FROM DUAL is standard.
            var sql = $"SELECT {sequenceName}.NEXTVAL FROM DUAL";
            // Using a temporary class/record for mapping raw SQL results can be more robust.
            // However, for a single value, mapping to long and casting is common.
            var result = await Database.SqlQueryRaw<long>(sql).ToListAsync(); // Use SingleAsync for a single expected value
            return (int)result[0];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Pais
            modelBuilder.Entity<Pais>(entity =>
            {
                entity.ToTable("t_gsab_pais");
                entity.HasKey(e => e.IdPais).HasName("pk_gsab_pais");
                entity.Property(e => e.IdPais).HasColumnName("id_pais").IsRequired();
                entity.Property(e => e.NmPais).HasColumnName("nm_pais").HasMaxLength(100).IsRequired();
            });

            // Estado
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("t_gsab_estado");
                entity.HasKey(e => e.IdEstado).HasName("pk_gsab_estado");
                entity.Property(e => e.IdEstado).HasColumnName("id_estado").IsRequired();
                entity.Property(e => e.NmEstado).HasColumnName("nm_estado").HasMaxLength(100).IsRequired();
                entity.Property(e => e.IdPais).HasColumnName("id_pais").IsRequired();
                /*entity.HasOne(d => d.Pais)
                    .WithMany(p => p.Estados)
                    .HasForeignKey(d => d.IdPais)
                    .OnDelete(DeleteBehavior.Restrict) // As per typical DB constraints, FKs usually restrict delete
                    .HasConstraintName("fk_estado_pais");*/
            });

            // Cidade
            modelBuilder.Entity<Cidade>(entity =>
            {
                entity.ToTable("t_gsab_cidade");
                entity.HasKey(e => e.IdCidade).HasName("pk_gsab_cidade");
                entity.Property(e => e.IdCidade).HasColumnName("id_cidade").IsRequired();
                entity.Property(e => e.NmCidade).HasColumnName("nm_cidade").HasMaxLength(100).IsRequired();
                entity.Property(e => e.IdEstado).HasColumnName("id_estado").IsRequired();
                /*entity.HasOne(d => d.Estado)
                    .WithMany(p => p.Cidades)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_cidade_estado");*/
            });

            // Endereco
            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.ToTable("t_gsab_endereco");
                entity.HasKey(e => e.IdEndereco).HasName("pk_gsab_endereco");
                entity.Property(e => e.IdEndereco).HasColumnName("id_endereco").IsRequired();
                entity.Property(e => e.DsCep).HasColumnName("ds_cep").HasMaxLength(11).IsRequired();
                entity.Property(e => e.DsLogradouro).HasColumnName("ds_logradouro").HasMaxLength(100).IsRequired();
                entity.Property(e => e.NrNumero).HasColumnName("nr_numero").IsRequired();
                entity.Property(e => e.DsComplemento).HasColumnName("ds_complemento").HasMaxLength(100).IsRequired(); // DDL: NOT NULL
                entity.Property(e => e.IdCidade).HasColumnName("id_cidade").IsRequired();
                /*entity.HasOne(d => d.Cidade)
                    .WithMany(p => p.Enderecos)
                    .HasForeignKey(d => d.IdCidade)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_endereco_cidade");*/
            });

            // Pessoa
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.ToTable("t_gsab_pessoa");
                entity.HasKey(e => e.IdPessoa).HasName("pk_gsab_pessoa");
                entity.Property(e => e.IdPessoa).HasColumnName("id_pessoa").IsRequired();
                entity.Property(e => e.NmPessoa).HasColumnName("nm_pessoa").HasMaxLength(100).IsRequired();
                entity.Property(e => e.NrCpf).HasColumnName("nr_cpf").HasMaxLength(14).IsRequired();
                entity.HasIndex(e => e.NrCpf).IsUnique().HasDatabaseName("uk_gsab_pessoa_cpf"); // Added unique constraint for CPF
                entity.Property(e => e.DtNascimento).HasColumnName("dt_nascimento").HasColumnType("DATE").IsRequired();
                entity.Property(e => e.DsCondicaoMedica).HasColumnName("ds_condicao_medica").HasColumnType("CLOB").IsRequired();
                entity.Property(e => e.StDesaparecido).HasColumnName("st_desaparecido").HasMaxLength(1).IsFixedLength().IsRequired();
                entity.Property(e => e.NmEmergencial).HasColumnName("nm_emergencial").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ContatoEmergencia).HasColumnName("contato_emergencia").HasMaxLength(100).IsRequired();
                entity.Property(e => e.IdEndereco).HasColumnName("id_endereco").IsRequired();
                /*entity.HasOne(d => d.Endereco)
                    .WithOne(p => p.Pessoa)
                    .HasForeignKey<Pessoa>(d => d.IdEndereco)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_pessoa_endereco");*/
            });

            // TipoUsuario
            modelBuilder.Entity<TipoUsuario>(entity =>
            {
                entity.ToTable("t_gsab_tipo_usuario");
                entity.HasKey(e => e.IdTipoUsuario).HasName("pk_tipo_usuario");
                entity.Property(e => e.IdTipoUsuario).HasColumnName("id_tipo_usuario").IsRequired();
                entity.Property(e => e.DsTipoUsuario).HasColumnName("ds_tipo_usuario").HasMaxLength(20).IsRequired();
            });

            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("t_gsab_usuario");
                entity.HasKey(e => e.IdUsuario).HasName("pk_usuario");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario").IsRequired();
                entity.Property(e => e.NmUsuario).HasColumnName("nm_usuario").HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.NmUsuario).IsUnique().HasDatabaseName("uk_gsab_usuario_nmusuario"); // Add unique constraint
                entity.Property(e => e.DsEmail).HasColumnName("ds_email").HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.DsEmail).IsUnique().HasDatabaseName("uk_gsab_usuario_dsemail"); // Add unique constraint
                entity.Property(e => e.DsSenha).HasColumnName("ds_senha").HasMaxLength(100).IsRequired();
                entity.Property(e => e.DsCodigoGoogle).HasColumnName("ds_codigo_google").HasMaxLength(120).IsRequired(); // DDL: NOT NULL
                entity.Property(e => e.IdTipoUsuario).HasColumnName("id_tipo_usuario").IsRequired();
                entity.Property(e => e.IdPessoa).HasColumnName("id_pessoa").IsRequired();
                entity.HasIndex(e => e.IdPessoa).IsUnique().HasDatabaseName("uk_gsab_usuario_idpessoa"); // One-to-one with Pessoa
                /*entity.HasOne(d => d.Pessoa)
                    .WithOne(p => p.Usuario)
                    .HasForeignKey<Usuario>(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.Cascade) // If Usuario is deleted, the link to Pessoa is via Pessoa's perspective. If Pessoa is deleted, Usuario should be deleted.
                    .HasConstraintName("fk_usuario_pessoa");
                entity.HasOne(d => d.TipoUsuario)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdTipoUsuario)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_usuario_tipo");*/
            });

            // Abrigo
            modelBuilder.Entity<Abrigo>(entity =>
            {
                entity.ToTable("t_gsab_abrigo");
                entity.HasKey(e => e.IdAbrigo).HasName("pk_gsab_abrigo");
                entity.Property(e => e.IdAbrigo).HasColumnName("id_abrigo").IsRequired();
                entity.Property(e => e.NmAbrigo).HasColumnName("nm_abrigo").HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.NmAbrigo).IsUnique().HasDatabaseName("uk_gsab_abrigo_nmabrigo"); // Add unique constraint
                entity.Property(e => e.NrCapacidade).HasColumnName("nr_capacidade").IsRequired();
                entity.Property(e => e.NrOcupacaoAtual).HasColumnName("nr_ocupacao_atual").IsRequired();
                entity.Property(e => e.IdEndereco).HasColumnName("id_endereco").IsRequired();
                /*entity.HasOne(d => d.Endereco)
                    .WithOne(p => p.Abrigo)
                    .HasForeignKey<Abrigo>(d => d.IdEndereco)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_abrigo_endereco");*/
            });

            // CheckIn
            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.ToTable("t_gsab_check_in");
                entity.HasKey(e => e.IdCheckin).HasName("pk_check_in");
                entity.Property(e => e.IdCheckin).HasColumnName("id_checkin").IsRequired();
                entity.Property(e => e.DtEntrada).HasColumnName("dt_entrada").HasColumnType("DATE").IsRequired();
                entity.Property(e => e.DtSaida).HasColumnName("dt_saida").HasColumnType("DATE"); // Nullable
                entity.Property(e => e.IdAbrigo).HasColumnName("id_abrigo").IsRequired();
                entity.Property(e => e.IdPessoa).HasColumnName("id_pessoa").IsRequired();
                /*entity.HasOne(d => d.Abrigo)
                    .WithMany(p => p.CheckIns)
                    .HasForeignKey(d => d.IdAbrigo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_checkin_abrigo");
                entity.HasOne(d => d.Pessoa)
                    .WithMany(p => p.CheckIns)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_checkin_pessoa");*/
            });

            // Recurso
            modelBuilder.Entity<Recurso>(entity =>
            {
                entity.ToTable("t_gsab_recurso");
                entity.HasKey(e => e.IdRecurso).HasName("pk_gsab_recurso");
                entity.Property(e => e.IdRecurso).HasColumnName("id_recurso").IsRequired();
                entity.Property(e => e.DsRecurso).HasColumnName("ds_recurso").HasMaxLength(100).IsRequired();
                entity.HasIndex(e => e.DsRecurso).IsUnique().HasDatabaseName("uk_gsab_recurso_dsrecurso"); // Add unique constraint
                entity.Property(e => e.QtPessoaDia).HasColumnName("qt_pessoa_dia").IsRequired();
                entity.Property(e => e.StConsumivel).HasColumnName("st_consumivel").HasMaxLength(1).IsFixedLength().IsRequired();
            });

            // EstoqueRecurso
            modelBuilder.Entity<EstoqueRecurso>(entity =>
            {
                entity.ToTable("t_gsab_estoque_recurso");
                entity.HasKey(e => e.IdEstoque).HasName("pk_estoque_recurso");
                entity.Property(e => e.IdEstoque).HasColumnName("id_estoque").IsRequired();
                entity.Property(e => e.QtDisponivel).HasColumnName("qt_disponivel").IsRequired();
                entity.Property(e => e.DtAtualizacao).HasColumnName("dt_atualizacao").HasColumnType("DATE").IsRequired();
                entity.Property(e => e.IdAbrigo).HasColumnName("id_abrigo").IsRequired();
                entity.Property(e => e.IdRecurso).HasColumnName("id_recurso").IsRequired();
                // Composite unique constraint for IdAbrigo and IdRecurso
                entity.HasIndex(e => new { e.IdAbrigo, e.IdRecurso }).IsUnique().HasDatabaseName("uk_gsab_estoque_abrigo_recurso");
                /*entity.HasOne(d => d.Abrigo)
                    .WithMany(p => p.EstoqueRecursos)
                    .HasForeignKey(d => d.IdAbrigo)
                    .OnDelete(DeleteBehavior.Cascade) // If Abrigo is deleted, its stock entries should also be deleted
                    .HasConstraintName("fk_estoque_abrigo");
                entity.HasOne(d => d.Recurso)
                    .WithMany(p => p.EstoqueRecursos)
                    .HasForeignKey(d => d.IdRecurso)
                    .OnDelete(DeleteBehavior.Restrict) // If Recurso is deleted, but it's in stock, prevent deletion
                    .HasConstraintName("fk_estoque_recurso");*/
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}

