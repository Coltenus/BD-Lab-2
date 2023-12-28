using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd_rgr
{
    public class VaccinesContext : BaseContext
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("vaccine_id")]
        public int VaccineId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Type { get; set; }
        public float Dosage { get; set; }
    }
    public class VaccinesModel : BaseModel
    {
        public enum VColumns : UInt16
        {
            Id = 1,
            Name,
            Manufacturer,
            Type,
            Dosage
        }
        public virtual DbSet<VaccinesContext> VContexts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VaccinesContext>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(v => v.VaccineId);
                entity.Property(e => e.Name)
                    .HasColumnName(TableFields[1])
                    .IsRequired();
                entity.Property(e => e.Manufacturer)
                    .HasColumnName(TableFields[2])
                    .IsRequired();
                entity.Property(e => e.Type)
                    .HasColumnName(TableFields[3])
                    .IsRequired();
                entity.Property(e => e.Dosage)
                    .HasColumnName(TableFields[4])
                    .IsRequired();
            });
        }

        public override UInt16 GetEnum(string name)
        {
            switch (name)
            {
                case "vaccination_id":
                    return (UInt16)VColumns.Id;
                case "name":
                    return (UInt16)VColumns.Name;
                case "manufacturer":
                    return (UInt16)VColumns.Manufacturer;
                case "type":
                    return (UInt16)VColumns.Type;
                case "dosage":
                    return (UInt16)VColumns.Dosage;
                default:
                    return 0;
            }
        }

        public override Dictionary<string, object> GetDict(BaseContext context)
        {
            var _context = (VaccinesContext)context;
            var dict = new Dictionary<string, object>();
            dict.Add(TableFields[0], _context.VaccineId);
            dict.Add(TableFields[1], _context.Name);
            dict.Add(TableFields[2], _context.Manufacturer);
            dict.Add(TableFields[3], _context.Type);
            dict.Add(TableFields[4], _context.Dosage);
            return dict;
        }

        public override BaseContext? Find(UInt16 column, object value)
        {
            var hwColumns = (VColumns)column;
            switch (hwColumns)
            {
                case VColumns.Id:
                    var id = (int)value;
                    return VContexts.FirstOrDefault(d=>d.VaccineId==id);
                case VColumns.Name:
                    var name = (string)value;
                    return VContexts.FirstOrDefault(d=>d.Name==name);
                case VColumns.Manufacturer:
                    var manufacturer = (string)value;
                    return VContexts.FirstOrDefault(d=>d.Manufacturer==manufacturer);
                case VColumns.Type:
                    var type = (string)value;
                    return VContexts.FirstOrDefault(d=>d.Type==type);
                case VColumns.Dosage:
                    var dosage = (float)value;
                    return VContexts.FirstOrDefault(d=>d.Dosage==dosage);
                default:
                    return new VaccinesContext();
            }
        }

        public override List<BaseContext> Find()
        {
            return VContexts.ToList<BaseContext>();
        }

        public override void Add(Dictionary<string, object> values)
        {
            try
            {
                int id = 1;
                foreach (var hw in VContexts)
                {
                    if (hw.VaccineId == id) id++;
                    else break;
                }
                VContexts.Add(new VaccinesContext()
                    { VaccineId = id, Name = (string)values["name"], Manufacturer = (string)values["manufacturer"],
                        Type = (string)values["type"], Dosage = (float)values["dosage"] });
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public override void Add(List<Dictionary<string, object>> values)
        {
            try
            {
                foreach(var value in values)
                {
                    int id = 1;
                    foreach (var hw in VContexts.OrderBy(d=>d.VaccineId))
                    {
                        if (hw.VaccineId == id) id++;
                        else break;
                    }
                    VContexts.Add(new VaccinesContext()
                        { VaccineId = id, Name = (string)value["name"], Manufacturer = (string)value["manufacturer"],
                            Type = (string)value["type"], Dosage = (float)value["dosage"] });
                    SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public override void Remove(UInt16 column, object value, int count = 1)
        {
            bool needSave = true;
            for(int i = 0; i<count || count == 0; i++)
            {
                var buf = (VaccinesContext?)Find(column, value);
                if (buf == null)
                {
                    if (i == 0) needSave = false;
                    Console.WriteLine("There is no such row.");
                    break;
                }
                VContexts.Remove(buf);
            }
            
            if(!needSave) return;
            try
            {
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public override void Remove(UInt16 column, object value, bool greater)
        {
            var hwColumn = (VColumns)column;
            foreach (var hw in VContexts)
            {
                bool remove = false;
                switch (hwColumn)
                {
                    case VColumns.Id:
                        if (greater && hw.VaccineId > (int)value || !greater && hw.VaccineId < (int)value)
                            remove = true;
                        break;
                    case VColumns.Name:
                        break;
                    case VColumns.Manufacturer:
                        break;
                    case VColumns.Type:
                        break;
                    case VColumns.Dosage:
                        if (greater && hw.Dosage > (float)value || !greater && hw.Dosage < (float)value)
                            remove = true;
                        break;
                }
                if(remove) VContexts.Remove(hw);
            }
            
            try
            {
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public override void Edit(UInt16 column, object value, Dictionary<UInt16, object> chVal, bool one = false)
        {
            var hwColumn = (VColumns)column;
            foreach (var hw in VContexts)
            {
                bool edit = false;
                switch (hwColumn)
                {
                    case VColumns.Id:
                        if (hw.VaccineId == (int)value)
                            edit = true;
                        break;
                    case VColumns.Name:
                        if (hw.Name == (string)value)
                            edit = true;
                        break;
                    case VColumns.Manufacturer:
                        if (hw.Manufacturer == (string)value)
                            edit = true;
                        break;
                    case VColumns.Type:
                        if (hw.Type == (string)value)
                            edit = true;
                        break;
                    case VColumns.Dosage:
                        if (hw.Dosage == (float)value)
                            edit = true;
                        break;
                }

                if (edit)
                {
                    bool needBreak = false;
                    for(var col = VColumns.Id; col<=VColumns.Type; col++)
                    {
                        UInt16 key = (UInt16)col;
                        if(!chVal.Keys.Contains(key)) continue;
                        switch (col)
                        {
                            case VColumns.Id:
                                Console.WriteLine("This column can not be modified.");
                                needBreak = true;
                                break;
                            case VColumns.Name:
                                hw.Name = (string)chVal[key];
                                break;
                            case VColumns.Manufacturer:
                                hw.Manufacturer = (string)chVal[key];
                                break;
                            case VColumns.Type:
                                hw.Type = (string)chVal[key];
                                break;
                            case VColumns.Dosage:
                                hw.Dosage = (float)chVal[key];
                                break;
                        }
                    }
                    if(one || needBreak) break;
                }
            }

            try
            {
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }
        //
        public VaccinesModel() : base()
        {
            TableName = "vaccines";
            TableFields = new List<string>()
            {
                "vaccine_id", "name", "manufacturer", "type", "dosage"
            };
        }
        
        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO vaccines\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS vaccine_id, CHR(TRUNC(65+RANDOM()*25)::int)" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"AS name, CHR(TRUNC(65+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"AS manufact, CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"AS type, TRUNC(RANDOM()*99.99 + 0.01)::float4 AS dosage " +
                          $"FROM GENERATE_SERIES({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.vaccine_id, t1.name, t1.manufact, t1.type, t1.dosage";
            
            if(debug)
                Console.WriteLine(command);
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = command;
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }
    }
}