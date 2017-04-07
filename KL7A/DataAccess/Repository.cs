using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using KL7A.Configuration;
using KL7A.Enums;

namespace KL7A.DataAccess
{
    internal static class Repository
    {
        private static YearlySettings GetYearlySettings(SqlDataReader rdr)
        {
            YearlySettings result = new YearlySettings();

            result.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
            result.Classification = rdr.GetString(rdr.GetOrdinal("Classification"));
            result.Date = rdr.GetDateTime(rdr.GetOrdinal("Date"));
            result.Indicator = rdr.GetString(rdr.GetOrdinal("Indicator"));
            result.Name = rdr.GetString(rdr.GetOrdinal("Name"));
            result.NumericIndicator = rdr.GetString(rdr.GetOrdinal("NumericIndicator"));

            return result;
        }
        private static MonthlySettings GetMonthlySettings(SqlDataReader rdr)
        {
            MonthlySettings result = new MonthlySettings();

            result.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
            result.Classification = rdr.GetString(rdr.GetOrdinal("Classification"));
            result.Date = rdr.GetDateTime(rdr.GetOrdinal("Date"));
            result.Indicator = rdr.GetString(rdr.GetOrdinal("Indicator"));
            result.Name = rdr.GetString(rdr.GetOrdinal("Name"));
            result.NumericIndicator = rdr.GetString(rdr.GetOrdinal("NumericIndicator"));
            result.ParentId = rdr.GetInt32(rdr.GetOrdinal("YearlySettingId"));

            return result;
        }
        private static Settings GetDailySettings(SqlDataReader rdr)
        {
            Settings result = new Settings();

            result.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
            result.Date = rdr.GetDateTime(rdr.GetOrdinal("Date"));
            result.Indicator = rdr.GetString(rdr.GetOrdinal("Indicator"));
            result.NumericIndicator = rdr.GetString(rdr.GetOrdinal("NumericIndicator"));
            result.Check = rdr.GetString(rdr.GetOrdinal("Check"));
            result.ParentId = rdr.GetInt32(rdr.GetOrdinal("MonthlySettingId"));
            result.StartPosition = rdr.GetString(rdr.GetOrdinal("StartPosition"));

            return result;
        }
        private static RotorSetting GetRotorSetting(SqlDataReader rdr)
        {
            RotorSetting result = new RotorSetting();

            result.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
            result.ParentId = rdr.GetInt32(rdr.GetOrdinal("DailySettingId"));
            result.RotorName = (RotorName)rdr.GetInt32(rdr.GetOrdinal("RotorName"));
            result.AlphabetRingPosition = rdr.GetInt32(rdr.GetOrdinal("AlphabetRingPosition"));
            result.NotchRingName = (NotchRingName)rdr.GetInt32(rdr.GetOrdinal("NotchRingName"));
            result.NotchRingPosition = rdr.GetInt32(rdr.GetOrdinal("NotchRingPosition"));

            return result;
        }
        private static Wiring GetWiring(SqlDataReader rdr)
        {
            Wiring result = new Wiring();

            result.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
            result.Plate1 = rdr.GetString(rdr.GetOrdinal("Plate1"));
            result.Plate2 = rdr.GetString(rdr.GetOrdinal("Plate2"));
            result.ParentId = rdr.GetInt32(rdr.GetOrdinal("MonthlyId"));

            return result;
        }
        private static void GetRotor(Dictionary<int, Wiring> lookup, SqlDataReader rdr)
        {
            lookup[rdr.GetInt32(0)].Rotors.Add(rdr.GetString(1));
        }
        private static void GetNotch(Dictionary<int, Wiring> lookup, SqlDataReader rdr)
        {
            lookup[rdr.GetInt32(0)].Notches.Add(rdr.GetString(1));
        }
        private static void GetMove(Dictionary<int, Wiring> lookup, SqlDataReader rdr)
        {
            lookup[rdr.GetInt32(0)].Moves.Add(rdr.GetString(1));
        }
        private static string GetYearlySettingsSql(int id, out SqlParameter parameter)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT YearlySetting.* FROM YearlySetting WHERE Id = @Id;");
            sb.AppendLine("SELECT MonthlySetting.* FROM MonthlySetting WHERE YearlySettingId = @Id ORDER BY MonthlySetting.Date;");
            sb.AppendLine("SELECT DailySetting.* FROM MonthlySetting JOIN DailySetting ON MonthlySetting.Id = DailySetting.MonthlySettingId WHERE MonthlySetting.YearlySettingId = @Id ORDER BY DailySetting.Date DESC;");
            sb.AppendLine("SELECT RotorSetting.* FROM RotorSetting JOIN DailySetting ON RotorSetting.DailySettingId = DailySetting.Id JOIN MonthlySetting ON DailySetting.MonthlySettingId = MonthlySetting.Id WHERE MonthlySetting.YearlySettingId = @Id ORDER BY RotorSetting.Ordinal;");
            sb.AppendLine("SELECT Wiring.* FROM Wiring JOIN MonthlySetting ON Wiring.MonthlyId = MonthlySetting.Id WHERE MonthlySetting.YearlySettingId = @Id ORDER BY MonthlySetting.Date;");
            sb.AppendLine("SELECT RotorWiringDefinition.WiringId, RotorWiringDefinition.Value FROM RotorWiringDefinition JOIN Wiring ON Wiring.Id = RotorWiringDefinition.WiringId JOIN MonthlySetting ON MonthlySetting.Id = Wiring.MonthlyId WHERE MonthlySetting.YearlySettingId = @Id ORDER BY RotorWiringDefinition.Ordinal;");
            sb.AppendLine("SELECT NotchDefinition.WiringId, NotchDefinition.Value FROM NotchDefinition JOIN Wiring ON Wiring.Id = NotchDefinition.WiringId JOIN MonthlySetting ON MonthlySetting.Id = Wiring.MonthlyId WHERE MonthlySetting.YearlySettingId = @Id ORDER BY NotchDefinition.Ordinal;");
            sb.AppendLine("SELECT MoveDefinition.WiringId, MoveDefinition.Value FROM MoveDefinition JOIN Wiring ON Wiring.Id = MoveDefinition.WiringId JOIN MonthlySetting ON MonthlySetting.Id = Wiring.MonthlyId WHERE MonthlySetting.YearlySettingId = @Id ORDER BY MoveDefinition.Ordinal;");

            parameter = new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = id };

            return sb.ToString();
        }

        private static string InsertYearlySettingsSql(YearlySettings ys, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            List<SqlParameter> p = new List<SqlParameter>();

            sb.AppendLine("INSERT INTO [dbo].[YearlySetting]");
            sb.AppendLine("           ([Name]");
            sb.AppendLine("           ,[Classification]");
            sb.AppendLine("           ,[Date]");
            sb.AppendLine("           ,[Indicator]");
            sb.AppendLine("           ,[NumericIndicator])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@Name");
            sb.AppendLine("           ,@Classification");
            sb.AppendLine("           ,@Date");
            sb.AppendLine("           ,@Indicator");
            sb.AppendLine("           ,@NumericIndicator);");
            sb.AppendLine("SELECT @Id = SCOPE_IDENTITY()");

            p.Add(new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output });
            p.Add(new SqlParameter { ParameterName = "@Name", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 255, Value = ys.Name });
            p.Add(new SqlParameter { ParameterName = "@Classification", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 255, Value = ys.Classification });
            p.Add(new SqlParameter { ParameterName = "@Date", DbType = System.Data.DbType.DateTime, Direction = System.Data.ParameterDirection.Input, Value = ys.Date });
            p.Add(new SqlParameter { ParameterName = "@Indicator", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 5, Value = ys.Indicator });
            p.Add(new SqlParameter { ParameterName = "@NumericIndicator", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 5, Value = ys.NumericIndicator });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertMonthlySettingsSql(MonthlySettings ms, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            List<SqlParameter> p = new List<SqlParameter>();

            sb.AppendLine("INSERT INTO [dbo].[MonthlySetting]");
            sb.AppendLine("           ([Name]");
            sb.AppendLine("           ,[Classification]");
            sb.AppendLine("           ,[Date]");
            sb.AppendLine("           ,[Indicator]");
            sb.AppendLine("           ,[YearlySettingId]");
            sb.AppendLine("           ,[NumericIndicator])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@Name");
            sb.AppendLine("           ,@Classification");
            sb.AppendLine("           ,@Date");
            sb.AppendLine("           ,@Indicator");
            sb.AppendLine("           ,@YearlySettingId");
            sb.AppendLine("           ,@NumericIndicator);");
            sb.AppendLine("SELECT @Id = SCOPE_IDENTITY()");

            p.Add(new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output });
            p.Add(new SqlParameter { ParameterName = "@Name", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 255, Value = ms.Name });
            p.Add(new SqlParameter { ParameterName = "@Classification", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 255, Value = ms.Classification });
            p.Add(new SqlParameter { ParameterName = "@Date", DbType = System.Data.DbType.DateTime, Direction = System.Data.ParameterDirection.Input, Value = ms.Date });
            p.Add(new SqlParameter { ParameterName = "@Indicator", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 5, Value = ms.Indicator });
            p.Add(new SqlParameter { ParameterName = "@YearlySettingId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = ms.ParentId });
            p.Add(new SqlParameter { ParameterName = "@NumericIndicator", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 5, Value = ms.NumericIndicator });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertDailySettingsSql(Settings s, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            List<SqlParameter> p = new List<SqlParameter>();

            sb.AppendLine("INSERT INTO[dbo].[DailySetting]");
            sb.AppendLine("           ([Date]");
            sb.AppendLine("           ,[Indicator]");
            sb.AppendLine("           ,[NumericIndicator]");
            sb.AppendLine("           ,[StartPosition]");
            sb.AppendLine("           ,[Check]");
            sb.AppendLine("           ,[MonthlySettingId])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@Date");
            sb.AppendLine("           ,@Indicator");
            sb.AppendLine("           ,@NumericIndicator");
            sb.AppendLine("           ,@StartPosition");
            sb.AppendLine("           ,@Check");
            sb.AppendLine("           ,@MonthlySettingId);");
            sb.AppendLine("SELECT @Id = SCOPE_IDENTITY()");

            p.Add(new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output });
            p.Add(new SqlParameter { ParameterName = "@Date", DbType = System.Data.DbType.DateTime, Direction = System.Data.ParameterDirection.Input, Value = s.Date });
            p.Add(new SqlParameter { ParameterName = "@Indicator", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 5, Value = s.Indicator });
            p.Add(new SqlParameter { ParameterName = "@NumericIndicator", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 5, Value = s.NumericIndicator });
            p.Add(new SqlParameter { ParameterName = "@StartPosition", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 8, Value = s.StartPosition });
            p.Add(new SqlParameter { ParameterName = "@Check", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 10, Value = s.Check });
            p.Add(new SqlParameter { ParameterName = "@MonthlySettingId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = s.ParentId });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertRotorSettingSql(RotorSetting rs, int i, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("INSERT INTO[dbo].[RotorSetting]");
            sb.AppendLine("           ([Ordinal]");
            sb.AppendLine("           ,[RotorName]");
            sb.AppendLine("           ,[AlphabetRingPosition]");
            sb.AppendLine("           ,[NotchRingName]");
            sb.AppendLine("           ,[NotchRingPosition]");
            sb.AppendLine("           ,[DailySettingId])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@Ordinal");
            sb.AppendLine("           ,@RotorName");
            sb.AppendLine("           ,@AlphabetRingPosition");
            sb.AppendLine("           ,@NotchRingName");
            sb.AppendLine("           ,@NotchRingPosition");
            sb.AppendLine("           ,@DailySettingId);");
            sb.AppendLine("SELECT @Id = SCOPE_IDENTITY()");

            List<SqlParameter> p = new List<SqlParameter>();

            p.Add(new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output });
            p.Add(new SqlParameter { ParameterName = "@Ordinal", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = i });
            p.Add(new SqlParameter { ParameterName = "@RotorName", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = rs.RotorName });
            p.Add(new SqlParameter { ParameterName = "@AlphabetRingPosition", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = rs.AlphabetRingPosition });
            p.Add(new SqlParameter { ParameterName = "@NotchRingName", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = rs.NotchRingName });
            p.Add(new SqlParameter { ParameterName = "@NotchRingPosition", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = rs.NotchRingPosition });
            p.Add(new SqlParameter { ParameterName = "@DailySettingId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = rs.ParentId });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertWiring(Wiring w, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("INSERT INTO[dbo].[Wiring]");
            sb.AppendLine("           ([Plate1]");
            sb.AppendLine("           ,[Plate2]");
            sb.AppendLine("           ,[MonthlyId])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@Plate1");
            sb.AppendLine("           ,@Plate2");
            sb.AppendLine("           ,@MonthlyId);");
            sb.AppendLine("SELECT @Id = SCOPE_IDENTITY()");

            List<SqlParameter> p = new List<SqlParameter>();

            p.Add(new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output });
            p.Add(new SqlParameter { ParameterName = "@Plate1", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 36, Value = w.Plate1 });
            p.Add(new SqlParameter { ParameterName = "@Plate2", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 36, Value = w.Plate2 });
            p.Add(new SqlParameter { ParameterName = "@MonthlyId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = w.ParentId });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertRotorWiringDef(int wiringId, int i, string value, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO[dbo].[RotorWiringDefinition]");
            sb.AppendLine("           ([WiringId]");
            sb.AppendLine("           ,[Ordinal]");
            sb.AppendLine("           ,[Value])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@WiringId");
            sb.AppendLine("           ,@Ordinal");
            sb.AppendLine("           ,@Value)");

            List<SqlParameter> p = new List<SqlParameter>();

            p.Add(new SqlParameter { ParameterName = "@WiringId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = wiringId });
            p.Add(new SqlParameter { ParameterName = "@Ordinal", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = i });
            p.Add(new SqlParameter { ParameterName = "@Value", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 36, Value = value });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertNotchRingDef(int wiringId, int i, string value, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO[dbo].[NotchDefinition]");
            sb.AppendLine("           ([WiringId]");
            sb.AppendLine("           ,[Ordinal]");
            sb.AppendLine("           ,[Value])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@WiringId");
            sb.AppendLine("           ,@Ordinal");
            sb.AppendLine("           ,@Value)");

            List<SqlParameter> p = new List<SqlParameter>();

            p.Add(new SqlParameter { ParameterName = "@WiringId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = wiringId });
            p.Add(new SqlParameter { ParameterName = "@Ordinal", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = i });
            p.Add(new SqlParameter { ParameterName = "@Value", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 36, Value = value });

            parameters = p.ToArray();

            return sb.ToString();
        }
        private static string InsertMoveDef(int wiringId, int i, string value, out SqlParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO[dbo].[MoveDefinition]");
            sb.AppendLine("           ([WiringId]");
            sb.AppendLine("           ,[Ordinal]");
            sb.AppendLine("           ,[Value])");
            sb.AppendLine("     VALUES");
            sb.AppendLine("           (@WiringId");
            sb.AppendLine("           ,@Ordinal");
            sb.AppendLine("           ,@Value)");

            List<SqlParameter> p = new List<SqlParameter>();

            p.Add(new SqlParameter { ParameterName = "@WiringId", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = wiringId });
            p.Add(new SqlParameter { ParameterName = "@Ordinal", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Input, Value = i });
            p.Add(new SqlParameter { ParameterName = "@Value", DbType = System.Data.DbType.String, Direction = System.Data.ParameterDirection.Input, Size = 36, Value = value });

            parameters = p.ToArray();

            return sb.ToString();
        }

        private static List<YearlySettings> GetYearlySettingsFromDb(SqlDataReader rdr)
        {
            List<YearlySettings> results = new List<YearlySettings>();

            if (rdr.Read())
            {
                YearlySettings result = GetYearlySettings(rdr);

                Dictionary<int, MonthlySettings> monthLookup = new Dictionary<int, MonthlySettings>();
                Dictionary<int, Settings> dayLookup = new Dictionary<int, Settings>();
                Dictionary<int, Wiring> wiringLookup = new Dictionary<int, Wiring>();

                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        MonthlySettings monSet = GetMonthlySettings(rdr);
                        monSet.ParentSettings = result;
                        result.MonthlySettings.Add(monSet);
                        monthLookup.Add(monSet.Id, monSet);
                    }
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        Settings daySet = GetDailySettings(rdr);
                        daySet.ParentSettings = monthLookup[daySet.ParentId];
                        monthLookup[daySet.ParentId].DailySettings.Add(daySet);
                        dayLookup.Add(daySet.Id, daySet);
                    }
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        RotorSetting rotSet = GetRotorSetting(rdr);
                        rotSet.ParentSettings = dayLookup[rotSet.ParentId];
                        dayLookup[rotSet.ParentId].Rotors.Add(rotSet);
                    }
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        Wiring wir = GetWiring(rdr);
                        wir.ParentSettings = monthLookup[wir.ParentId];
                        monthLookup[wir.ParentId].Wiring = wir;
                        wiringLookup.Add(wir.Id, wir);
                    }
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        GetRotor(wiringLookup, rdr);
                    }
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        GetNotch(wiringLookup, rdr);
                    }
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        GetMove(wiringLookup, rdr);
                    }
                }

                results.Add(result);
            }
            return results;
        }

        public static YearlySettings ReadFromDb(int yearlyId)
        {
            using (Database db = Database.Get())
            {
                SqlParameter parameter;
                string sql = GetYearlySettingsSql(yearlyId, out parameter);
                List<YearlySettings> result = db.ExecuteReader(sql, GetYearlySettingsFromDb, parameter);
                return result.FirstOrDefault();
            }
        }
        public static void SaveToDb(YearlySettings yr)
        {
            using (Database db = Database.Get())
            {
                try
                {
                    db.BeginTransaction();

                    SqlParameter[] parameters;
                    string sql = InsertYearlySettingsSql(yr, out parameters);
                    db.ExecuteNonQuery(sql, parameters);
                    yr.Id = (int)parameters.FirstOrDefault(p => p.ParameterName == "@Id").Value;

                    foreach (MonthlySettings monSet in yr.MonthlySettings)
                    {
                        monSet.ParentId = yr.Id;

                        sql = InsertMonthlySettingsSql(monSet, out parameters);
                        db.ExecuteNonQuery(sql, parameters);
                        monSet.Id = (int)parameters.FirstOrDefault(p => p.ParameterName == "@Id").Value;

                        foreach (Settings daily in monSet.DailySettings)
                        {
                            daily.ParentId = monSet.Id;

                            sql = InsertDailySettingsSql(daily, out parameters);
                            db.ExecuteNonQuery(sql, parameters);
                            daily.Id = (int)parameters.FirstOrDefault(p => p.ParameterName == "@Id").Value;

                            for (int i = 0; i < daily.Rotors.Count; i++)
                            {
                                daily.Rotors[i].ParentId = daily.Id;

                                sql = InsertRotorSettingSql(daily.Rotors[i], i, out parameters);
                                db.ExecuteNonQuery(sql, parameters);
                            }
                        }

                        monSet.Wiring.ParentId = monSet.Id;

                        sql = InsertWiring(monSet.Wiring, out parameters);
                        db.ExecuteNonQuery(sql, parameters);
                        monSet.Wiring.Id = (int)parameters.FirstOrDefault(p => p.ParameterName == "@Id").Value;

                        for (int i = 0; i < monSet.Wiring.Rotors.Count; i++)
                        {
                            sql = InsertRotorWiringDef(monSet.Wiring.Id, i, monSet.Wiring.Rotors[i], out parameters);
                            db.ExecuteNonQuery(sql, parameters);
                        }
                        for (int i = 0; i < monSet.Wiring.Notches.Count; i++)
                        {
                            sql = InsertNotchRingDef(monSet.Wiring.Id, i, monSet.Wiring.Notches[i], out parameters);
                            db.ExecuteNonQuery(sql, parameters);
                        }
                        for (int i = 0; i < monSet.Wiring.Moves.Count; i++)
                        {
                            sql = InsertMoveDef(monSet.Wiring.Id, i, monSet.Wiring.Moves[i], out parameters);
                            db.ExecuteNonQuery(sql, parameters);
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    throw ex;
                }
            }
        }
    }
}
