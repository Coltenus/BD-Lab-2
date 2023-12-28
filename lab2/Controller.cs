#nullable enable
using System.Text.RegularExpressions;

namespace bd_rgr
{
    public class Controller
    {
        private BaseModel? _model;
        
        public Controller()
        {
            BaseModel.Connection = new Connection("localhost", "lab1", "postgres", "1111");
            _model = null;
        }

        public void Run()
        {
            bool active = true;
            while(active)
            {
                try
                {
                    active = ChooseAction();
                }
                catch (Exception ex)
                {
                    View.ShowException(ex);
                }
            }
        }

        private bool ChooseAction()
        {
            bool active = true;
            int action = Convert.ToInt32(View.RequestInput("Choose action: "));
            try
            {
                switch (action)
                {
                    case 0:
                        active = false;
                        break;
                    case 1:
                        ChooseModel();
                        break;
                    case 2:
                    {
                        var debug = Convert.ToBoolean(View.RequestInput("Need debug(true, false): "));
                        var data = new List<Tuple<string, string, string>>();
                        Regex regexData = new Regex(@"\w+");
                        while (true)
                        {
                            var dt = View.RequestInput("Enter data(table name, needed column, column for join)\n: ");
                            if (dt == string.Empty) break;
                            var _data = regexData.Matches(dt);
                            if (_data.Count < 3) continue;
                            data.Add(new Tuple<string, string, string>(_data[0].Value, _data[1].Value, _data[2].Value));
                        }

                        var columns = new List<string>();
                        foreach (var item in data)
                        {
                            columns.Add(item.Item2);
                        }

                        Tuple<int, string, List<object>> where;
                        {
                            var table = Convert.ToInt32(View.RequestInput("Enter count of table: "));
                            var column = View.RequestInput("Enter column: ");
                            List<object> values = new List<object>();
                            while (true)
                            {
                                var value = View.RequestInput("Enter value: ");
                                if (value == string.Empty)
                                    break;
                                values.Add(value);
                            }

                            where = new Tuple<int, string, List<object>>(table, column, values);
                        }
                        View.PrintDictList(_model.FindInTables(data, where, debug), columns);
                    }
                        break;
                    case 3:
                    {
                        var column = View.RequestInput("Enter column: ");
                        var value = View.RequestInput("Enter value: ");
                        View.PrintDict(_model.GetDict(_model.Find(_model.GetEnum(column), value)), _model.TableFields);
                    }
                        break;
                    case 4:
                    {
                        var column = View.RequestInput("Enter column: ");
                        List<object> values = new List<object>();
                        while (true)
                        {
                            var value = View.RequestInput("Enter value: ");
                            if (value == string.Empty)
                                break;
                            values.Add(value);
                        }

                        View.PrintDictList(_model.Find(column, values), _model.TableFields);
                    }
                        break;
                    case 5:
                        View.PrintDictList(_model.GetDict(_model.Find()), _model.TableFields);
                        break;
                    case 6:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var field in _model.TableFields)
                        {
                            var value = View.RequestInput($"Enter {field}: ");
                            dict.Add(field, value);
                        }

                        _model.Add(dict);
                    }
                        break;
                    case 7:
                    {
                        var list = new List<Dictionary<string, object>>();
                        while(true)
                        {
                            bool end = false;
                            var dict = new Dictionary<string, object>();
                            foreach (var field in _model.TableFields)
                            {
                                var value = View.RequestInput($"Enter {field}: ");
                                if (value == string.Empty)
                                {
                                    end = true;
                                    break;
                                }
                                dict.Add(field, value);
                            }
                            if(end) break;
                            list.Add(dict);
                        }

                        _model.Add(list);
                    }
                        break;
                    case 8:
                    {
                        var column = View.RequestInput("Enter column: ");
                        var value = View.RequestInput("Enter value: ");
                        _model.Remove(_model.GetEnum(column), value);
                    }
                        break;
                    case 9:
                    {
                        var column = View.RequestInput("Enter column: ");
                        var value = View.RequestInput("Enter value: ");
                        var greater = Convert.ToBoolean(View.RequestInput("Greater or less(true, false): "));
                        _model.Remove(_model.GetEnum(column), int.Parse(value), greater);
                    }
                        break;
                    case 10:
                    {
                        var columnEdit = View.RequestInput("Enter column to find: ");
                        var valueEdit = View.RequestInput("Enter its value: ");
                        var values = new Dictionary<UInt16, object>();
                        View.GetValues1(ref values, in _model);
                        _model.Edit(_model.GetEnum(columnEdit), valueEdit, values, true);
                    }
                        break;
                    case 11:
                    {
                        var columnEdit = View.RequestInput("Enter column to find: ");
                        var valueEdit = View.RequestInput("Enter its value: ");
                        var values = new Dictionary<UInt16, object>();
                        View.GetValues1(ref values, in _model);
                        _model.Edit(_model.GetEnum(columnEdit), valueEdit, values);
                    }
                        break;
                    case 12:
                    {
                        var count = Convert.ToUInt32(View.RequestInput("Enter count of elements: "));
                        var debug = Convert.ToBoolean(View.RequestInput("Need debug(true, false): "));
                        _model.GenerateSeries(count, debug);
                    }
                        break;
                }
            }
            catch (NullReferenceException ex)
            {
                View.ShowException(ex, "You need to choose model to use it.");
            }
            
            System.Threading.Thread.Sleep(100);
            return active;
        }

        private void ChooseModel()
        {
            int action = Convert.ToInt32(View.RequestInput("Choose model: "));
            switch (action)
            {
                case 1:
                    _model = new PatientsModel();
                    break;
                case 2:
                    _model = new VaccinesModel();  
                    break;
                case 3:
                    _model = new VaccinationsModel();
                    break;
                case 4:
                    _model = new HealthcareWorkersModel();
                    break;
                case 5:
                    _model = new VaccinationScheduleModel();
                    break;
            }
        }
    }
}