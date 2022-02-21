using ESMA.ChangesCloser;
using ESMA.DataCollections.CoreDataCollections;
using System;
using System.ComponentModel;

namespace ESMA
{
    public interface IData
    {
        public static DateTime StartDateValue { get; set; }
        public static DateTime EndDateValue { get; set; }

        public static MainWindow Window { get; set; }
        public static ConferenceSettingsWindow CsWindow { get; set; }
        public static NewTaskWindow NtWindow { get; set; }

        public static BindingList<VideoConference> VideoConferences { get; set; }
        public static BindingList<Changes> Changes { get; set; }
        public static BindingList<Process> Processes { get; set; }
        public static BindingList<ChangesCreate> CTCs { get; set; }
        public static BindingList<PlanCoordinator> PCs { get; set; }
        public static BindingList<ChangesCloserElement> ChangesCloserElements { get; set; }
    }
}
