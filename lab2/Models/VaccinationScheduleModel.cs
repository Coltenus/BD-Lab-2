using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd_rgr
{
    public class VaccinationScheduleContext : BaseContext
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("schedule_id")]
        public int ScheduleId { get; set; }
        public int WorkerID { get; set; }
        public int VaccinationID { get; set; }
        public DateTime Date { get; set; }
        
    }
    public class VaccinationScheduleModel : BaseModel
    {
        public enum VSColumns : UInt16
        {
            Id = 1,
            WorkerID,
            VaccinationID,
            Date,
        }
        public virtual DbSet<VaccinationScheduleContext> VSContexts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VaccinationScheduleContext>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(v => v.ScheduleId);
                entity.HasOne<HealthCareWorkersContext>()
                    .WithMany().HasForeignKey(v => v.WorkerID);
                entity.Property(v => v.WorkerID)
                    .HasColumnName(TableFields[1]);
                entity.HasOne<VaccinationsContext>()
                    .WithMany().HasForeignKey(v => v.VaccinationID);
                entity.Property(v => v.VaccinationID)
                    .HasColumnName(TableFields[2]);
                entity.Property(e => e.Date)
                    .HasColumnName(TableFields[3])
                    .IsRequired();
            });
        }

        public override UInt16 GetEnum(string name)
        {
            switch (name)
            {
                case "patient_id":
                    return (UInt16)VSColumns.Id;
                case "worker_id":
                    return (UInt16)VSColumns.WorkerID;
                case "vaccination_id":
                    return (UInt16)VSColumns.VaccinationID;
                case "date":
                    return (UInt16)VSColumns.Date;
                default:
                    return 0;
            }
        }

        public override Dictionary<string, object> GetDict(BaseContext context)
        {
            var _context = (VaccinationScheduleContext)context;
            var dict = new Dictionary<string, object>();
            dict.Add(TableFields[0], _context.ScheduleId);
            dict.Add(TableFields[1], _context.WorkerID);
            dict.Add(TableFields[2], _context.VaccinationID);
            dict.Add(TableFields[3], _context.Date);
            return dict;
        }

        public override BaseContext? Find(UInt16 column, object value)
        {
            var hwColumns = (VSColumns)column;
            switch (hwColumns)
            {
                case VSColumns.Id:
                    var id = (int)value;
                    return VSContexts.FirstOrDefault(d=>d.ScheduleId==id);
                case VSColumns.WorkerID:
                    var workerId = (int)value;
                    return VSContexts.FirstOrDefault(d=>d.WorkerID==workerId);
                case VSColumns.VaccinationID:
                    var vaccinationId = (int)value;
                    return VSContexts.FirstOrDefault(d=>d.VaccinationID==vaccinationId);
                case VSColumns.Date:
                    var date = (DateTime)value;
                    return VSContexts.FirstOrDefault(d=>d.Date==date);
                default:
                    return new VaccinationScheduleContext();
            }
        }

        public override List<BaseContext> Find()
        {
            return VSContexts.ToList<BaseContext>();
        }

        public override void Add(Dictionary<string, object> values)
        {
            try
            {
                int id = 1;
                foreach (var hw in VSContexts)
                {
                    if (hw.ScheduleId == id) id++;
                    else break;
                }
                VSContexts.Add(new VaccinationScheduleContext()
                    { ScheduleId = id, WorkerID = (int)values["worker_id"], VaccinationID = (int)values["vaccination_id"],
                        Date = (DateTime)values["date"] });
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
                    foreach (var hw in VSContexts.OrderBy(d=>d.ScheduleId))
                    {
                        if (hw.ScheduleId == id) id++;
                        else break;
                    }
                    VSContexts.Add(new VaccinationScheduleContext()
                        { ScheduleId = id, WorkerID = (int)value["worker_id"], VaccinationID = (int)value["vaccination_id"],
                            Date = (DateTime)value["date"] });
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
                var buf = (VaccinationScheduleContext?)Find(column, value);
                if (buf == null)
                {
                    if (i == 0) needSave = false;
                    Console.WriteLine("There is no such row.");
                    break;
                }
                VSContexts.Remove(buf);
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
            var hwColumn = (VSColumns)column;
            foreach (var hw in VSContexts)
            {
                bool remove = false;
                switch (hwColumn)
                {
                    case VSColumns.Id:
                        if (greater && hw.ScheduleId > (int)value || !greater && hw.ScheduleId < (int)value)
                            remove = true;
                        break;
                    case VSColumns.WorkerID:
                        if (greater && hw.WorkerID > (int)value || !greater && hw.WorkerID < (int)value)
                            remove = true;
                        break;
                    case VSColumns.VaccinationID:
                        if (greater && hw.VaccinationID > (int)value || !greater && hw.VaccinationID < (int)value)
                            remove = true;
                        break;
                    case VSColumns.Date:
                        if (greater && hw.Date > (DateTime)value || !greater && hw.Date < (DateTime)value)
                            remove = true;
                        break;
                }
                if(remove) VSContexts.Remove(hw);
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
            var hwColumn = (VSColumns)column;
            foreach (var hw in VSContexts)
            {
                bool edit = false;
                switch (hwColumn)
                {
                    case VSColumns.Id:
                        if (hw.ScheduleId == (int)value)
                            edit = true;
                        break;
                    case VSColumns.WorkerID:
                        if (hw.WorkerID == (int)value)
                            edit = true;
                        break;
                    case VSColumns.VaccinationID:
                        if (hw.VaccinationID == (int)value)
                            edit = true;
                        break;
                    case VSColumns.Date:
                        if (hw.Date == (DateTime)value)
                            edit = true;
                        break;
                }

                if (edit)
                {
                    bool needBreak = false;
                    for(var col = VSColumns.Id; col<=VSColumns.Date; col++)
                    {
                        UInt16 key = (UInt16)col;
                        if(!chVal.Keys.Contains(key)) continue;
                        switch (col)
                        {
                            case VSColumns.Id:
                                Console.WriteLine("This column can not be modified.");
                                needBreak = true;
                                break;
                            case VSColumns.WorkerID:
                                hw.WorkerID = (int)chVal[key];
                                break;
                            case VSColumns.VaccinationID:
                                hw.VaccinationID = (int)chVal[key];
                                break;
                            case VSColumns.Date:
                                hw.Date = (DateTime)chVal[key];
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
        public VaccinationScheduleModel() : base()
        {
            TableName = "vaccination_schedule";
            TableFields = new List<string>()
            {
                "schedule_id", "worker_id", "vaccination_id", "date"
            };
        }
        
        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO vaccination_schedule\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS schedule_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(worker_id) FROM healthcare_workers))::int + 1 AS worker_id," +
                          $"TRUNC(RANDOM()*((SELECT MAX(vaccination_id) FROM vaccinations)))::int + 1 AS vaccination_id," +
                          $"DATE('2023-01-01') + TRUNC(RANDOM()*(DATE('2023-12-31') - DATE('2023-01-01')))::int AS date " +
                          $"FROM generate_series({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.schedule_id, t1.worker_id, t1.vaccination_id, t1.date";
            
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