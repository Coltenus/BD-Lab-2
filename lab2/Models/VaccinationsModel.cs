using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd_rgr
{
    public class VaccinationsContext : BaseContext
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("vaccination_id")]
        public int VaccinationId { get; set; }
        public int PatientID { get; set; }
        public int VaccineID { get; set; }
        public float GivenDose { get; set; }
        public string? Notes { get; set; }
    }
    public class VaccinationsModel : BaseModel
    {
        public enum VColumns : UInt16
        {
            Id = 1,
            PatientID,
            VaccineID,
            GivenDose,
            Notes
        }
        public virtual DbSet<VaccinationsContext> VContexts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VaccinationsContext>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(v => v.VaccinationId);
                entity.HasOne<PatientsContext>()
                    .WithMany().HasForeignKey(v => v.PatientID);
                entity.Property(v => v.PatientID)
                    .HasColumnName(TableFields[1]);
                entity.HasOne<VaccinesContext>()
                    .WithMany().HasForeignKey(v => v.VaccineID);
                entity.Property(v => v.VaccineID)
                    .HasColumnName(TableFields[2]);
                entity.Property(e => e.GivenDose)
                    .HasColumnName(TableFields[3])
                    .IsRequired();
                entity.Property(e => e.Notes)
                    .HasColumnName(TableFields[4]);
            });
        }

        public override UInt16 GetEnum(string name)
        {
            switch (name)
            {
                case "vaccination_id":
                    return (UInt16)VColumns.Id;
                case "patient_id":
                    return (UInt16)VColumns.PatientID;
                case "vaccine_id":
                    return (UInt16)VColumns.VaccineID;
                case "given_dose":
                    return (UInt16)VColumns.GivenDose;
                case "notes":
                    return (UInt16)VColumns.Notes;
                default:
                    return 0;
            }
        }

        public override Dictionary<string, object> GetDict(BaseContext context)
        {
            var _context = (VaccinationsContext)context;
            var dict = new Dictionary<string, object>();
            dict.Add(TableFields[0], _context.VaccinationId);
            dict.Add(TableFields[1], _context.PatientID);
            dict.Add(TableFields[2], _context.VaccineID);
            dict.Add(TableFields[3], _context.GivenDose);
            dict.Add(TableFields[4], _context.Notes);
            return dict;
        }

        public override BaseContext? Find(UInt16 column, object value)
        {
            var hwColumns = (VColumns)column;
            switch (hwColumns)
            {
                case VColumns.Id:
                    var id = (int)value;
                    return VContexts.FirstOrDefault(d=>d.VaccinationId==id);
                case VColumns.PatientID:
                    var patientId = (int)value;
                    return VContexts.FirstOrDefault(d=>d.PatientID==patientId);
                case VColumns.VaccineID:
                    var vaccineId = (int)value;
                    return VContexts.FirstOrDefault(d=>d.VaccineID==vaccineId);
                case VColumns.GivenDose:
                    var givenDose = (float)value;
                    return VContexts.FirstOrDefault(d=>d.GivenDose==givenDose);
                case VColumns.Notes:
                    var notes = (string?)value;
                    return VContexts.FirstOrDefault(d=>d.Notes==notes);
                default:
                    return new VaccinationsContext();
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
                    if (hw.VaccinationId == id) id++;
                    else break;
                }
                VContexts.Add(new VaccinationsContext()
                    { VaccinationId = id, PatientID = (int)values["patient_id"], VaccineID = (int)values["vaccine_id"],
                        GivenDose = (float)values["given_dose"], Notes = (string?)values["notes"] });
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
                    foreach (var hw in VContexts.OrderBy(d=>d.VaccinationId))
                    {
                        if (hw.VaccinationId == id) id++;
                        else break;
                    }
                    VContexts.Add(new VaccinationsContext()
                        { VaccinationId = id, PatientID = (int)value["patient_id"], VaccineID = (int)value["vaccine_id"],
                            GivenDose = (float)value["given_dose"], Notes = (string?)value["notes"] });
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
                var buf = (VaccinationsContext?)Find(column, value);
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
                        if (greater && hw.VaccinationId > (int)value || !greater && hw.VaccinationId < (int)value)
                            remove = true;
                        break;
                    case VColumns.PatientID:
                        if (greater && hw.PatientID > (int)value || !greater && hw.PatientID < (int)value)
                            remove = true;
                        break;
                    case VColumns.VaccineID:
                        if (greater && hw.VaccineID > (int)value || !greater && hw.VaccineID < (int)value)
                            remove = true;
                        break;
                    case VColumns.GivenDose:
                        if (greater && hw.GivenDose > (float)value || !greater && hw.GivenDose < (float)value)
                            remove = true;
                        break;
                    case VColumns.Notes:
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
                        if (hw.VaccinationId == (int)value)
                            edit = true;
                        break;
                    case VColumns.PatientID:
                        if (hw.PatientID == (int)value)
                            edit = true;
                        break;
                    case VColumns.VaccineID:
                        if (hw.VaccineID == (int)value)
                            edit = true;
                        break;
                    case VColumns.GivenDose:
                        if (hw.GivenDose == (float)value)
                            edit = true;
                        break;
                    case VColumns.Notes:
                        if (hw.Notes == (string?)value)
                            edit = true;
                        break;
                }

                if (edit)
                {
                    bool needBreak = false;
                    for(var col = VColumns.Id; col<=VColumns.GivenDose; col++)
                    {
                        UInt16 key = (UInt16)col;
                        if(!chVal.Keys.Contains(key)) continue;
                        switch (col)
                        {
                            case VColumns.Id:
                                Console.WriteLine("This column can not be modified.");
                                needBreak = true;
                                break;
                            case VColumns.PatientID:
                                hw.PatientID = (int)chVal[key];
                                break;
                            case VColumns.VaccineID:
                                hw.VaccineID = (int)chVal[key];
                                break;
                            case VColumns.GivenDose:
                                hw.GivenDose = (float)chVal[key];
                                break;
                            case VColumns.Notes:
                                hw.Notes = (string?)chVal[key];
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
        public VaccinationsModel() : base()
        {
            TableName = "vaccinations";
            TableFields = new List<string>()
            {
                "vaccination_id", "patient_id", "vaccine_id", "given_dose", "notes"
            };
        }
        
        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO vaccinations\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS vaccination_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(patient_id) FROM patients))::int + 1 AS patient_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(vaccine_id) FROM vaccines))::int + 1 AS vaccine_id," +
                          $"float4(RANDOM()*0.99 + 0.01) AS dosage " +
                          $"FROM GENERATE_SERIES({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.vaccination_id, t1.patient_id, t1.vaccine_id, t1.dosage";
            
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