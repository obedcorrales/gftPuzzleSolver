using System;
using System.Collections.Generic;

namespace dF.Commons.Proxies.DirtyPropertiesTrackerProxy
{
    [Serializable]
    public class DirtyPropertiesTracker : IDirtyPropertiesTracker
    {
        private IList<string> __dirtyPropertiesNameList { get; } = new List<string>();

        public bool IsTrackingAllPropertyChanges { get; private set; } = true;

        public bool IsUsingDefaultsToRemoveTrackedProperties { get; private set; } = false;

        public IList<string> GetDirtyPropertiesNameList()
        {
            return __dirtyPropertiesNameList;
        }

        public void TrackAllPropertyChanges()
        {
            IsTrackingAllPropertyChanges = true;
            IsUsingDefaultsToRemoveTrackedProperties = false;
        }

        public void UseDefaultsToRemoveTrackedProperties()
        {
            IsTrackingAllPropertyChanges = false;
            IsUsingDefaultsToRemoveTrackedProperties = true;
        }

        public void DontUseDefaultsToRemoveTrackedProperties()
        {
            IsUsingDefaultsToRemoveTrackedProperties = false;
        }

        public void StopTrackingAllPropertyChanges()
        {
            IsTrackingAllPropertyChanges = false;
        }
    }
}
