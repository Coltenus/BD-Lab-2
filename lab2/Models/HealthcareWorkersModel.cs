using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd_rgr
{
    public class HealthCareWorkersContext : BaseContext
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("worker_id")]
        public int WorkerId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public long MedicalLicenseNumber { get; set; }
        
    }
    public class HealthcareWorkersModel : BaseModel
    {
        public enum HWColumns : UInt16
        {
            Id = 1,
            Name,
            Specialization,
            MedicalLicenseNumber,
        }
        public virtual DbSet<HealthCareWorkersContext> HwContexts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HealthCareWorkersContext>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(h => h.WorkerId);
                entity.Property(e => e.Name)
                    .HasColumnName(TableFields[1])
                    .IsRequired();
                entity.Property(e => e.Specialization)
                    .HasColumnName(TableFields[2])
                    .IsRequired();
                entity.Property(e => e.MedicalLicenseNumber)
                    .HasColumnName(TableFields[3])
                    .IsRequired();
            });
        }

        public override UInt16 GetEnum(string name)
        {
            switch (name)
            {
                case "worker_id":
                    return (UInt16)HWColumns.Id;
                case "name":
                    return (UInt16)HWColumns.Name;
                case "specialization":
                    return (UInt16)HWColumns.Specialization;
                case "medical_license_number":
                    return (UInt16)HWColumns.MedicalLicenseNumber;
                default:
                    return 0;
            }
        }

        public override Dictionary<string, object> GetDict(BaseContext context)
        {
            var _context = (HealthCareWorkersContext)context;
            var dict = new Dictionary<string, object>();
            dict.Add(TableFields[0], _context.WorkerId);
            dict.Add(TableFields[1], _context.Name);
            dict.Add(TableFields[2], _context.Specialization);
            dict.Add(TableFields[3], _context.MedicalLicenseNumber);
            return dict;
        }

        public override BaseContext? Find(UInt16 column, object value)
        {
            var hwColumns = (HWColumns)column;
            switch (hwColumns)
            {
                case HWColumns.Id:
                    int id = (int)value;
                    return HwContexts.FirstOrDefault(d=>d.WorkerId==id);
                case HWColumns.Name:
                    string name = (string)value;
                    return HwContexts.FirstOrDefault(d=>d.Name==name);
                case HWColumns.Specialization:
                    string spec = (string)value;
                    return HwContexts.FirstOrDefault(d=>d.Specialization==spec);
                case HWColumns.MedicalLicenseNumber:
                    long mln = (long)value;
                    return HwContexts.FirstOrDefault(d=>d.MedicalLicenseNumber==mln);
                default:
                    return new HealthCareWorkersContext();
            }
        }

        public override List<BaseContext> Find()
        {
            return HwContexts.ToList<BaseContext>();
        }

        public override void Add(Dictionary<string, object> values)
        {
            try
            {
                int id = 1;
                foreach (var hw in HwContexts)
                {
                    if (hw.WorkerId == id) id++;
                    else break;
                }
                HwContexts.Add(new HealthCareWorkersContext()
                    { WorkerId = id, Name = (string)values["name"], Specialization = (string)values["specialization"],
                        MedicalLicenseNumber = (long)values["medical_license_number"] });
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
                    foreach (var hw in HwContexts.OrderBy(d=>d.WorkerId))
                    {
                        if (hw.WorkerId == id) id++;
                        else break;
                    }
                    HwContexts.Add(new HealthCareWorkersContext()
                        { WorkerId = id, Name = (string)value["name"], Specialization = (string)value["specialization"],
                            MedicalLicenseNumber = (long)value["medical_license_number"] });
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
                var buf = (HealthCareWorkersContext?)Find(column, value);
                if (buf == null)
                {
                    if (i == 0) needSave = false;
                    Console.WriteLine("There is no such row.");
                    break;
                }
                HwContexts.Remove(buf);
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
            var hwColumn = (HWColumns)column;
            foreach (var hw in HwContexts)
            {
                bool remove = false;
                switch (hwColumn)
                {
                    case HWColumns.Id:
                        if (greater && hw.WorkerId > (int)value || !greater && hw.WorkerId < (int)value)
                            remove = true;
                        break;
                    case HWColumns.Name:
                        break;
                    case HWColumns.Specialization:
                        break;
                    case HWColumns.MedicalLicenseNumber:
                        if (greater && hw.MedicalLicenseNumber > (long)value
                            || !greater && hw.MedicalLicenseNumber < (long)value)
                            remove = true;
                        break;
                }
                if(remove) HwContexts.Remove(hw);
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
            var hwColumn = (HWColumns)column;
            foreach (var hw in HwContexts)
            {
                bool edit = false;
                switch (hwColumn)
                {
                    case HWColumns.Id:
                        if (hw.WorkerId == (int)value)
                            edit = true;
                        break;
                    case HWColumns.Name:
                        if (hw.Name == (string)value)
                            edit = true;
                        break;
                    case HWColumns.Specialization:
                        if (hw.Specialization == (string)value)
                            edit = true;
                        break;
                    case HWColumns.MedicalLicenseNumber:
                        if (hw.MedicalLicenseNumber == (long)value)
                            edit = true;
                        break;
                }

                if (edit)
                {
                    bool needBreak = false;
                    for(var col = HWColumns.Id; col<=HWColumns.MedicalLicenseNumber; col++)
                    {
                        UInt16 key = (UInt16)col;
                        if(!chVal.Keys.Contains(key)) continue;
                        switch (col)
                        {
                            case HWColumns.Id:
                                Console.WriteLine("This column can not be modified.");
                                needBreak = true;
                                break;
                            case HWColumns.Name:
                                hw.Name = (string)chVal[key];
                                break;
                            case HWColumns.Specialization:
                                hw.Specialization = (string)chVal[key];
                                break;
                            case HWColumns.MedicalLicenseNumber:
                                Console.WriteLine("This column can not be modified.");
                                needBreak = true;
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
        public HealthcareWorkersModel() : base()
        {
            TableName = "healthcare_workers";
            TableFields = new List<string>()
            {
                "worker_id", "name", "specialization", "medical_license_number"
            };
        }
        
        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO healthcare_workers\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS worker_id, CHR(TRUNC(65+RANDOM()*25)::int)" +
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
                          $"AS spec, TRUNC(RANDOM()*99999999999999)::int8 AS license " +
                          $"FROM generate_series({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.worker_id, t1.name, t1.spec, t1.license";
            
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