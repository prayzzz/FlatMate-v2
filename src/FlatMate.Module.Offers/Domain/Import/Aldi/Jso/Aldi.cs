using System.Collections.Generic;
using System.Xml.Serialization;

namespace FlatMate.Module.Offers.Domain.Import.Aldi.Jso
{
    [XmlRoot(ElementName = "area")]
    public class Area
    {
        [XmlElement(ElementName = "area_image")]
        public string AreaImage { get; set; }

        [XmlElement(ElementName = "area_image_retina")]
        public string AreaImageRetina { get; set; }

        [XmlElement(ElementName = "area_image_tablet")]
        public string AreaImageTablet { get; set; }

        [XmlElement(ElementName = "area_image_tablet_retina")]
        public string AreaImageTabletRetina { get; set; }

        [XmlElement(ElementName = "articles")]
        public Articles Articles { get; set; }

        [XmlAttribute(AttributeName = "rel")]
        public string Rel { get; set; }

        [XmlElement(ElementName = "teasers")]
        public Teasers Teasers { get; set; }

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
    }

    [XmlRoot(ElementName = "article")]
    public class Article
    {
        [XmlElement(ElementName = "articleid")]
        public string Articleid { get; set; }

        [XmlElement(ElementName = "articlexml")]
        public string Articlexml { get; set; }

        [XmlElement(ElementName = "badges")]
        public Badges Badges { get; set; }

        [XmlElement(ElementName = "catrel")]
        public string Catrel { get; set; }

        [XmlElement(ElementName = "footnote")]
        public string Footnote { get; set; }

        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }

        [XmlElement(ElementName = "noprice")]
        public string Noprice { get; set; }

        [XmlElement(ElementName = "pack_subtitle")]
        public string PackSubtitle { get; set; }

        [XmlElement(ElementName = "pack_timestamp")]
        public string PackTimestamp { get; set; }

        [XmlElement(ElementName = "pack_timestamp_actiondate")]
        public string PackTimestampActiondate { get; set; }

        [XmlElement(ElementName = "pack_timestamp_end")]
        public string PackTimestampEnd { get; set; }

        [XmlElement(ElementName = "pack_timestamp_remove")]
        public string PackTimestampRemove { get; set; }

        [XmlElement(ElementName = "pack_timestamp_start")]
        public string PackTimestampStart { get; set; }

        [XmlElement(ElementName = "pack_title")]
        public string PackTitle { get; set; }

        [XmlElement(ElementName = "price")]
        public string Price { get; set; }

        [XmlElement(ElementName = "price_calc")]
        public string PriceCalc { get; set; }

        [XmlElement(ElementName = "price_extra")]
        public string PriceExtra { get; set; }

        [XmlElement(ElementName = "price_old")]
        public string PriceOld { get; set; }

        [XmlElement(ElementName = "producer")]
        public string Producer { get; set; }

        [XmlElement(ElementName = "shorttext")]
        public string Shorttext { get; set; }

        [XmlElement(ElementName = "subtitle")]
        public string Subtitle { get; set; }

        [XmlElement(ElementName = "teaserxml")]
        public string Teaserxml { get; set; }

        [XmlElement(ElementName = "text")]
        public string Text { get; set; }

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "weburl")]
        public string Weburl { get; set; }
    }

    [XmlRoot(ElementName = "articles")]
    public class Articles
    {
        [XmlElement(ElementName = "article")]
        public List<Article> Article { get; set; }
    }

    [XmlRoot(ElementName = "badge")]
    public class Badge
    {
        [XmlElement(ElementName = "additional_text")]
        public string AdditionalText { get; set; }

        [XmlElement(ElementName = "image_normal")]
        public ImageNormal ImageNormal { get; set; }

        [XmlElement(ElementName = "image_retina")]
        public ImageRetina ImageRetina { get; set; }

        [XmlElement(ElementName = "target")]
        public string Target { get; set; }

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "badges")]
    public class Badges
    {
        [XmlElement(ElementName = "badge")]
        public List<Badge> Badge { get; set; }
    }

    [XmlRoot(ElementName = "config")]
    public class Config
    {
        [XmlElement(ElementName = "baseurl")]
        public string Baseurl { get; set; }

        [XmlElement(ElementName = "webbaseurl")]
        public string Webbaseurl { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        [XmlElement(ElementName = "area")]
        public List<Area> Area { get; set; }

        [XmlElement(ElementName = "config")]
        public Config Config { get; set; }

        [XmlElement(ElementName = "html")]
        public string Html { get; set; }
    }

    [XmlRoot(ElementName = "image_normal")]
    public class ImageNormal
    {
        [XmlAttribute(AttributeName = "height")]
        public string Height { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }
    }

    [XmlRoot(ElementName = "image_retina")]
    public class ImageRetina
    {
        [XmlAttribute(AttributeName = "height")]
        public string Height { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }
    }

    [XmlRoot(ElementName = "images")]
    public class Images
    {
        [XmlElement(ElementName = "img")]
        public List<Img> Img { get; set; }
    }

    [XmlRoot(ElementName = "img")]
    public class Img
    {
        [XmlElement(ElementName = "detail_normal")]
        public string DetailNormal { get; set; }

        [XmlElement(ElementName = "detail_retina")]
        public string DetailRetina { get; set; }

        [XmlElement(ElementName = "overview_normal")]
        public string OverviewNormal { get; set; }

        [XmlElement(ElementName = "overview_retina")]
        public string OverviewRetina { get; set; }

        [XmlElement(ElementName = "shoplist_normal")]
        public string ShoplistNormal { get; set; }

        [XmlElement(ElementName = "shoplist_retina")]
        public string ShoplistRetina { get; set; }

        [XmlElement(ElementName = "slider_normal")]
        public string SliderNormal { get; set; }

        [XmlElement(ElementName = "slider_retina")]
        public string SliderRetina { get; set; }

        [XmlElement(ElementName = "thumb_normal")]
        public string ThumbNormal { get; set; }

        [XmlElement(ElementName = "thumb_retina")]
        public string ThumbRetina { get; set; }
    }

    [XmlRoot(ElementName = "teaser")]
    public class Teaser
    {
        [XmlElement(ElementName = "article")]
        public Article Article { get; set; }

        [XmlElement(ElementName = "catrel")]
        public string Catrel { get; set; }

        [XmlElement(ElementName = "teaserxml")]
        public string Teaserxml { get; set; }
    }

    [XmlRoot(ElementName = "teasers")]
    public class Teasers
    {
        [XmlElement(ElementName = "teaser")]
        public List<Teaser> Teaser { get; set; }
    }
}