using System.Collections.Generic;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    public interface IDirtyPropertiesTracker
    {
        bool IsTrackingAllPropertyChanges { get; }
        bool IsUsingDefaultsToRemoveTrackedProperties { get; }

        IList<string> GetDirtyPropertiesNameList();
        void UseDefaultsToRemoveTrackedProperties();
        void DontUseDefaultsToRemoveTrackedProperties();
        void TrackAllPropertyChanges();
        void StopTrackingAllPropertyChanges();
    }
}
