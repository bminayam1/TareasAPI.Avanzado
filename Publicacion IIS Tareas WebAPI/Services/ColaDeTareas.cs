using System.Collections.Generic;
using System;
using TareasAPI.Models;
using TareasAPI.Factories;

namespace TareasAPI.Services
{
    public class ColaDeTareas<T>
    {
        private readonly Queue<T> _tareas = new Queue<T>();
        private bool _procesando = false;

        public void AgregarTarea(T tarea)
        {
            _tareas.Enqueue(tarea);
            Console.WriteLine($"Tarea agregada: {tarea}");

            // Si no hay ninguna tarea en proceso, inicia el procesamiento  
            if (!_procesando)
            {
                _procesando = true;
                _ = ProcesarTareasAsync();
            }
        }

        private async Task ProcesarTareasAsync()
        {
            while (_tareas.Count > 0)
            {
                T tareasActual = _tareas.Dequeue();
                Console.WriteLine($"Procesando tarea: {tareasActual}");

                await Task.Delay(2000); // Simula el tiempo de procesamiento de la tarea  
                Console.WriteLine($"Tarea procesada: {tareasActual}");
            }
            _procesando = false;
        }
    }
}
