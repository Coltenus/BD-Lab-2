using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd_rgr
{
    public class PatientsContext : BaseContext
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("patient_id")]
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        
    }
    public class PatientsModel : BaseModel
    {
        public enum PColumns : UInt16
        {
            Id = 1,
            Name,
            Address,
            PhoneNumber,
        }
        public virtual DbSet<PatientsContext> PContexts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientsContext>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(p => p.PatientId);
                entity.Property(e => e.Name)
                    .HasColumnName(TableFields[1])
                    .IsRequired();
                entity.Property(e => e.Address)
                    .HasColumnName(TableFields[2])
                    .IsRequired();
                entity.Property(e => e.PhoneNumber)
                    .HasColumnName(TableFields[3])
                    .IsRequired();
            });
        }

        public override UInt16 GetEnum(string name)
        {
            switch (name)
            {
                case "patient_id":
                    return (UInt16)PColumns.Id;
                case "name":
                    return (UInt16)PColumns.Name;
                case "address":
                    return (UInt16)PColumns.Address;
                case "phone_number":
                    return (UInt16)PColumns.PhoneNumber;
                default:
                    return 0;
            }
        }

        public override Dictionary<string, object> GetDict(BaseContext context)
        {
            var _context = (PatientsContext)context;
            var dict = new Dictionary<string, object>();
            dict.Add(TableFields[0], _context.PatientId);
            dict.Add(TableFields[1], _context.Name);
            dict.Add(TableFields[2], _context.Address);
            dict.Add(TableFields[3], _context.PhoneNumber);
            return dict;
        }

        public override BaseContext? Find(UInt16 column, object value)
        {
            var hwColumns = (PColumns)column;
            switch (hwColumns)
            {
                case PColumns.Id:
                    var id = (int)value;
                    return PContexts.FirstOrDefault(d=>d.PatientId==id);
                case PColumns.Name:
                    var name = (string)value;
                    return PContexts.FirstOrDefault(d=>d.Name==name);
                case PColumns.Address:
                    var address = (string)value;
                    return PContexts.FirstOrDefault(d=>d.Address==address);
                case PColumns.PhoneNumber:
                    var ph = (string)value;
                    return PContexts.FirstOrDefault(d=>d.PhoneNumber==ph);
                default:
                    return new PatientsContext();
            }
        }

        public override List<BaseContext> Find()
        {
            return PContexts.ToList<BaseContext>();
        }

        public override void Add(Dictionary<string, object> values)
        {
            try
            {
                int id = 1;
                foreach (var hw in PContexts)
                {
                    if (hw.PatientId == id) id++;
                    else break;
                }
                PContexts.Add(new PatientsContext()
                    { PatientId = id, Name = (string)values["name"], Address = (string)values["address"],
                        PhoneNumber = (string)values["phone_number"] });
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
                    foreach (var hw in PContexts.OrderBy(d=>d.PatientId))
                    {
                        if (hw.PatientId == id) id++;
                        else break;
                    }
                    PContexts.Add(new PatientsContext()
                        { PatientId = id, Name = (string)value["name"], Address = (string)value["address"],
                            PhoneNumber = (string)value["phone_number"] });
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
                var buf = (PatientsContext?)Find(column, value);
                if (buf == null)
                {
                    if (i == 0) needSave = false;
                    Console.WriteLine("There is no such row.");
                    break;
                }
                PContexts.Remove(buf);
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
            var hwColumn = (PColumns)column;
            foreach (var hw in PContexts)
            {
                bool remove = false;
                switch (hwColumn)
                {
                    case PColumns.Id:
                        if (greater && hw.PatientId > (int)value || !greater && hw.PatientId < (int)value)
                            remove = true;
                        break;
                    case PColumns.Name:
                        break;
                    case PColumns.Address:
                        break;
                    case PColumns.PhoneNumber:
                        break;
                }
                if(remove) PContexts.Remove(hw);
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
            var hwColumn = (PColumns)column;
            foreach (var hw in PContexts)
            {
                bool edit = false;
                switch (hwColumn)
                {
                    case PColumns.Id:
                        if (hw.PatientId == (int)value)
                            edit = true;
                        break;
                    case PColumns.Name:
                        if (hw.Name == (string)value)
                            edit = true;
                        break;
                    case PColumns.Address:
                        if (hw.Address == (string)value)
                            edit = true;
                        break;
                    case PColumns.PhoneNumber:
                        if (hw.PhoneNumber == (string)value)
                            edit = true;
                        break;
                }

                if (edit)
                {
                    bool needBreak = false;
                    for(var col = PColumns.Id; col<=PColumns.PhoneNumber; col++)
                    {
                        UInt16 key = (UInt16)col;
                        if(!chVal.Keys.Contains(key)) continue;
                        switch (col)
                        {
                            case PColumns.Id:
                                Console.WriteLine("This column can not be modified.");
                                needBreak = true;
                                break;
                            case PColumns.Name:
                                hw.Name = (string)chVal[key];
                                break;
                            case PColumns.Address:
                                hw.Address = (string)chVal[key];
                                break;
                            case PColumns.PhoneNumber:
                                hw.PhoneNumber = (string)chVal[key];
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
        public PatientsModel() : base()
        {
            TableName = "patients";
            TableFields = new List<string>()
            {
                "patient_id", "name", "address", "phone_number"
            };
        }

        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO patients\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS patient_id, CHR(TRUNC(65+RANDOM()*25)::int)" +
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
                          $"AS address, '+' || CHR(TRUNC(48+RANDOM()*10)::int)\n" +
                          $"|| CHR(TRUNC(48+RANDOM()*10)::int) || CHR(TRUNC(48+RANDOM()*10)::int)\n" +
                          $"|| CHR(TRUNC(48+RANDOM()*10)::int) || CHR(TRUNC(48+RANDOM()*10)::int)\n" +
                          $"|| CHR(TRUNC(48+RANDOM()*10)::int)\n" +
                          $"AS phone_number FROM generate_series({max_id+1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.patient_id, t1.name, t1.address, t1.phone_number";
            
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