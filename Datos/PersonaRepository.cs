using Entidad;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Datos
{
    public class PersonaRepository
    {
        string ruta = "Persona.txt";
        DbConnection _connection;
        public PersonaRepository(DbConnection connection)
        {
            _connection = connection;
        }
       
        
        public void Guardar(Persona persona)
        {
            using (var command = _connection.CreateCommand())
            {
                //command.CommandText = "insert into persona (Identificacion, Edad, Pulsacion , sexo, Nombre) values (@Identificacion, @Edad, @Pulsacion, @Sexo, @Nombre )";
                command.CommandText = $"insert into persona (Identificacion, Edad, Pulsacion , sexo, Nombre) values ('{persona.Identificacion}', {persona.Edad}, {persona.Pulsacion}, '{persona.Sexo}', '{persona.Nombre}')";
                //command.Parameters.Add(new SqlParameter("@Identificacion", persona.Identificacion));
                //command.Parameters.Add(new SqlParameter("@Edad", persona.Edad));
                //command.Parameters.Add(new SqlParameter("@Pulsacion", persona.Pulsacion));
                //command.Parameters.Add(new SqlParameter("@Sexo", persona.Sexo));
                //command.Parameters.Add(new SqlParameter("@Nombre", persona.Nombre));
                int fila=command.ExecuteNonQuery();

            }
        }
        public List<Persona> Consultar()
        {
            List<Persona> personas = new List<Persona>();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "select * from persona";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Persona persona = new Persona();
                    persona.Identificacion = reader.GetString(0);
                    persona.Nombre = reader.GetString(1);
                    persona.Sexo = reader["Sexo"].ToString();
                    persona.Edad = reader.GetInt32(3);
                    personas.Add(persona);
                }
                reader.Close();
            }
            
            return personas;
        }

        private static Persona MapearPersona(string linea)
        {
            string[] datosPersona = linea.Split(';');
            Persona persona = new Persona();
            persona.Identificacion = datosPersona[0];
            persona.Nombre = datosPersona[1];
            persona.Sexo = datosPersona[2];
            persona.Edad = Int32.Parse(datosPersona[3]);
            persona.Pulsacion = Convert.ToDecimal(datosPersona[4]);
            return persona;
        }

        public void Eliminar(string identificacion)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "delete From Persona where Identificacion=@Identificacion";
                command.Parameters.Add(new SqlParameter("@Identificacion", identificacion));
                int fila = command.ExecuteNonQuery();

            }

        }

        public void Modificar(Persona personaNuevo, string identificacion)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = "update persona set Nombre=@Nombre, Edad=@Edad,Sexo=@Sexo, pulsacion=@Pulsacion where Identificacion=@Identificacion";
                    command.Parameters.Add(new SqlParameter("@Identificacion", identificacion));
                    command.Parameters.Add(new SqlParameter("@Edad", personaNuevo.Edad));
                    command.Parameters.Add(new SqlParameter("@Pulsacion", personaNuevo.Pulsacion));
                    command.Parameters.Add(new SqlParameter("@Sexo", personaNuevo.Sexo));
                    command.Parameters.Add(new SqlParameter("@Nombre", personaNuevo.Nombre));
                    int fila = command.ExecuteNonQuery();
                    transaction.Commit();
                }
            } 
        }

        public Persona BuscarPorIdentificacion(string identificacion)
        {
           
            using (var command = _connection.CreateCommand())
            {
                command.CreateParameter();
                command.CommandText = "select * from persona where Identificacion=:Identificacion";
                //command.Parameters.Add(new OracleParameter(":identificacion",identificacion));
                CreateParameter(command, "identificacion", identificacion);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Persona persona = new Persona();
                        persona.Identificacion = reader.GetString(0);
                        persona.Nombre = reader.GetString(1);
                        persona.Sexo = reader["Sexo"].ToString();
                        persona.Edad = reader.GetInt32(3);
                        return persona;
                    }
                }
                
                reader.Close();
            }
            return null;
        }

        public void CreateParameter(DbCommand command, string parameterName, object value)
        {
            DbParameter paramametro = command.CreateParameter();
            paramametro.ParameterName = parameterName;
            paramametro.Value = value;
            command.Parameters.Add(paramametro);
        }

        public List<Persona> FiltrarPorSexo(string sexo)
        {
            return (from p in Consultar()
                   where p.Sexo.Equals(sexo)
                   orderby p.Nombre ascending
                   select p).ToList();
        }

        public List<Persona> FiltrarPorSexoMetodo(string sexo)
        {
            return Consultar().Where(p=>p.Sexo.Equals(sexo)).ToList();
        }

        public int Contar (string sexo)
        {
            return Consultar().Count(p=>p.Sexo.Equals(sexo));
        }

        public double Promediar()
        {
            return Consultar().Average(p => p.Edad);
        }


        public int Sumar()
        {
            return Convert.ToInt32(Consultar().Sum(p=>p.Pulsacion));
        }


        public List<Persona> FiltrarPorAnio(int year, int month)
        {
            return Consultar().Where(p=>p.FechaNacimiento.Year==year && p.FechaNacimiento.Month== month).ToList();
        }


        public List<Persona> FiltrarPorPalabra(string palabra)
        {
            return (from p in Consultar()
                    where p.Nombre.ToLower().Contains(palabra.ToLower())
                    select p).ToList();
        }
        public List<Persona> FiltrarPorSexoVersionLarga (string sexo)
        {
            List<Persona> personasFiltradas = new List<Persona>();
            foreach (var item in Consultar())
            {
                if (item.Sexo.Equals(sexo))
                {
                    personasFiltradas.Add(item);
                }
            }
            return personasFiltradas;
        }

    }
}
