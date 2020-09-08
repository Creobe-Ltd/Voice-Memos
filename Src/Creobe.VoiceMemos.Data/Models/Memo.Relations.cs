using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Creobe.VoiceMemos.Data.Models
{
    public partial class Memo
    {
        #region Tags Relational Property

        private Collection<Tag> _tags;

		[XmlIgnore]
        public Collection<Tag> Tags
        {
            get { return _tags ?? (_tags = CreateTagsRelation()); }
        }

        ObservableCollection<Tag> CreateTagsRelation()
        {
            var tags = TagsFK == null ? Enumerable.Empty<Tag>() : VoiceMemosDatabase.Tags.LoadByKeys(TagsFK);
            var result = new ObservableCollection<Tag>(tags);
            result.CollectionChanged += (s, e) => SyncTagsFK();
            return result;
        }

        void SyncTagsFK()
        {
            TagsFK = (from i in _tags select i.Id).ToArray();
        }

        #endregion

        #region Markers Relational Property

        private Collection<Marker> _markers;

        [XmlIgnore]
        public Collection<Marker> Markers
        {
            get { return _markers ?? (_markers = CreateMarkersRelation()); }
        }

        ObservableCollection<Marker> CreateMarkersRelation()
        {
            var markers = MarkersFK == null ? Enumerable.Empty<Marker>() : VoiceMemosDatabase.Markers.LoadByKeys(MarkersFK);
            var result = new ObservableCollection<Marker>(markers);
            result.CollectionChanged += (s, e) => SyncMarkersFK();
            return result;
        }

        void SyncMarkersFK()
        {
            MarkersFK = (from i in _markers select i.Id).ToArray();
        }

        #endregion

    }
}
