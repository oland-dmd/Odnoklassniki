namespace Oland.Odnoklassniki.Rest.BeanFields;

public static class MediaTopicBeanFields
{
            /// <summary>Признак административного продвижения (буста) записи</summary>
        public const string AdmBoostPost = "adm_boost_post";
        /// <summary>URL полноэкранного рекламного холста (canvas)</summary>
        public const string AdCanvasUrl = "ad_canvas_url";
        /// <summary>Метаданные рекламного объявления</summary>
        public const string AdInfo = "ad_info";
        /// <summary>Признак товара из интеграции с AliExpress</summary>
        public const string AliExpressProduct = "ali_express_product";
        /// <summary>Данные о юбилейном событии или специальной дате</summary>
        public const string Anniversary = "anniversary";
        /// <summary>Ссылка или идентификатор приложения-источника поста</summary>
        public const string AppRef = "app_ref";
        /// <summary>Расширенная ссылка на внешнее приложение или игру</summary>
        public const string AppRefExt = "app_ref_ext";
        /// <summary>Вложения поста (фото, видео, товары, ссылки)</summary>
        public const string Attachment = "attachment";
        /// <summary>Ссылка на автора или публикатора записи</summary>
        public const string AuthorRef = "author_ref";
        /// <summary>Данные о привязанном челлендже или флешмобе</summary>
        public const string Challenge = "challenge";
        /// <summary>Разрешено ли изменение тематического тега/хобби</summary>
        public const string ChangeHobbyAllowed = "change_hobby_allowed";
        /// <summary>Метаданные комментариев к посту</summary>
        public const string CommentRefs = "comment_refs";
        /// <summary>Время создания поста в миллисекундах (UNIX timestamp)</summary>
        public const string CreatedMs = "created_ms";
        /// <summary>Идентификатор декоратора или шаблона оформления</summary>
        public const string DecoratorId = "decorator_id";
        /// <summary>Внутренний идентификатор для операции удаления</summary>
        public const string DeleteId = "delete_id";
        /// <summary>Сводка по обсуждениям и веткам комментариев</summary>
        public const string DiscussionSummary = "discussion_summary";
        /// <summary>Разрешено ли редактирование поста текущим пользователем</summary>
        public const string EditAllowed = "edit_allowed";
        /// <summary>Признак проверки на недостоверную информацию или фейк</summary>
        public const string FakeNews = "fake_news";
        /// <summary>Разрешено ли вступление в группу напрямую из поста</summary>
        public const string GroupJoinAllowed = "group_join_allowed";
        /// <summary>Флаг наличия рекламного холста в записи</summary>
        public const string HasAdCanvas = "has_ad_canvas";
        /// <summary>Флаг наличия лид-формы для сбора заявок</summary>
        public const string HasLeadAds = "has_lead_ads";
        /// <summary>Флаг наличия дополнительных записей для пагинации</summary>
        public const string HasMore = "has_more";
        /// <summary>Тематика или хобби, привязанное к посту</summary>
        public const string Hobby = "hobby";
        /// <summary>Расширенная информация о тематике/рубрике</summary>
        public const string HobbyInfo = "hobby_info";
        /// <summary>Данные о праздничном оформлении или событии</summary>
        public const string HolidayEvent = "holiday_event";
        /// <summary>Уникальный идентификатор поста или записи в ленте</summary>
        public const string Id = "id";
        /// <summary>Информация о нарушениях или несоответствиях правилам</summary>
        public const string Inconsistency = "inconsistency";
        /// <summary>Флаг того, что пост является рекламным объявлением</summary>
        public const string IsAdsPost = "is_ads_post";
        /// <summary>Флаг объявления об услуге (а не физическом товаре)</summary>
        public const string IsAdForService = "is_ad_for_service";
        /// <summary>Флаг возможности онлайн-оплаты через Маркет</summary>
        public const string IsAdSoldOnline = "is_ad_sold_online";
        /// <summary>Флаг отзыва или рецензии о бизнес-профиле</summary>
        public const string IsBusinessProfileReview = "is_business_profile_review";
        /// <summary>Запрещены ли комментарии к этой записи</summary>
        public const string IsCommentingDenied = "is_commenting_denied";
        /// <summary>Доступно ли редактирование в текущий момент</summary>
        public const string IsEditable = "is_editable";
        /// <summary>Флаг поста с эмоцией или настроением пользователя</summary>
        public const string IsFeeling = "is_feeling";
        /// <summary>Скрыт ли пост от публичной ленты</summary>
        public const string IsHiddenPost = "is_hidden_post";
        /// <summary>Флаг модерируемого баннера или промо-блока</summary>
        public const string IsModeratedBanner = "is_moderated_banner";
        /// <summary>Опубликовано ли от имени сообщества/группы</summary>
        public const string IsOnBehalfOfGroup = "is_on_behalf_of_group";
        /// <summary>Флаг того, что пост содержит товарную карточку</summary>
        public const string IsProduct = "is_product";
        /// <summary>Флаг специальной промо-акции или скидки</summary>
        public const string IsPromo = "is_promo";
        /// <summary>Опубликовано ли как бесплатное объявление</summary>
        public const string IsPublishedAsFree = "is_published_as_free";
        /// <summary>Закреплен ли пост в верхней части ленты или раздела</summary>
        public const string IsSticky = "is_sticky";
        /// <summary>Обрезан ли текст поста (требует раскрытия "Подробнее")</summary>
        public const string IsTextCut = "is_text_cut";
        /// <summary>Полностью ли запрещено любое изменение записи</summary>
        public const string IsUnmodifiable = "is_unmodifiable";
        /// <summary>Черновик или неопубликованная запись</summary>
        public const string IsUnpublishedPost = "is_unpublished_post";
        /// <summary>Пост из раздела "Карапули" (детский контент)</summary>
        public const string KarapuliaPost = "karapulia_post";
        /// <summary>Время последнего отклонения модерацией (timestamp)</summary>
        public const string LastRejectTimeMs = "last_reject_time_ms";
        /// <summary>Сводка по лайкам, реакциям и отметкам "нравится"</summary>
        public const string LikeSummary = "like_summary";
        /// <summary>Признак формата длинной статьи или лонгрида</summary>
        public const string Longread = "longread";
        /// <summary>Товар из официального маркетплейса OK Mall</summary>
        public const string MallProduct = "mall_product";
        /// <summary>Идентификатор для отправки жалобы на спам</summary>
        public const string MarkAsSpamId = "mark_as_spam_id";
        /// <summary>Основной медиа-контейнер вложения поста</summary>
        public const string Media = "media";
        /// <summary>Ссылка на действие или callback приложения</summary>
        public const string MediaAppActLink = "media_app_act_link";
        /// <summary>Изображение, связанное с приложением</summary>
        public const string MediaAppImage = "media_app_image";
        /// <summary>Маркировка или рейтинг приложения</summary>
        public const string MediaAppMark = "media_app_mark";
        /// <summary>Ссылка на приложение внутри медиа-карточки</summary>
        public const string MediaAppRef = "media_app_ref";
        /// <summary>Текстовое описание от приложения</summary>
        public const string MediaAppText = "media_app_text";
        /// <summary>Заголовок приложения в медиа-блоке</summary>
        public const string MediaAppTitle = "media_app_title";
        /// <summary>Кнопки быстрых действий в медиа-карточке</summary>
        public const string MediaButtons = "media_buttons";
        /// <summary>Уникальный ключ или идентификатор кнопки</summary>
        public const string MediaButtonKey = "media_button_key";
        /// <summary>Отображаемый текст на кнопке</summary>
        public const string MediaButtonTitle = "media_button_title";
        /// <summary>Тип кнопки (ссылка, покупка, подписка, действие)</summary>
        public const string MediaButtonType = "media_button_type";
        /// <summary>Карусель изображений или товарных карточек</summary>
        public const string MediaCarousel = "media_carousel";
        /// <summary>Описание медиа-вложения или превью</summary>
        public const string MediaDescription = "media_description";
        /// <summary>Домен-источник внешнего медиа-контента</summary>
        public const string MediaDomain = "media_domain";
        /// <summary>Обложка группы в медиа-представлении</summary>
        public const string MediaGroupCover = "media_group_cover";
        /// <summary>Метаданные группы-владельца медиа</summary>
        public const string MediaGroupData = "media_group_data";
        /// <summary>Ссылки на тематические рубрики или категории</summary>
        public const string MediaMediaTopicRefs = "media_media_topic_refs";
        /// <summary>Ссылки на фильмы, сериалы или видеоконтент</summary>
        public const string MediaMovieRefs = "media_movie_refs";
        /// <summary>Ссылки на музыкальные плейлисты</summary>
        public const string MediaMusicPlaylistRefs = "media_music_playlist_refs";
        /// <summary>Ссылки на отдельные музыкальные треки</summary>
        public const string MediaMusicTrackRefs = "media_music_track_refs";
        /// <summary>Ссылки на фотографии во вложении</summary>
        public const string MediaPhotoRefs = "media_photo_refs";
        /// <summary>Ссылки на опросы или голосования</summary>
        public const string MediaPollRefs = "media_poll_refs";
        /// <summary>Ссылки на виртуальные подарки</summary>
        public const string MediaPresentRefs = "media_present_refs";
        /// <summary>Данные о репосте или шеринге записи</summary>
        public const string MediaReshare = "media_reshare";
        /// <summary>Данные автора оригинальной записи при репосте</summary>
        public const string MediaReshareOwnerRefs = "media_reshare_owner_refs";
        /// <summary>Элементы товарной витрины в медиа-блоке</summary>
        public const string MediaShowcaseItems = "media_showcase_items";
        /// <summary>Текстовое содержимое медиа-карточки</summary>
        public const string MediaText = "media_text";
        /// <summary>Токенизированный текст для парсинга упоминаний и ссылок</summary>
        public const string MediaTextTokens = "media_text_tokens";
        /// <summary>Заголовок медиа-вложения</summary>
        public const string MediaTitle = "media_title";
        /// <summary>Тип медиа (photo, video, product, link, carousel и т.д.)</summary>
        public const string MediaType = "media_type";
        /// <summary>Основной URL медиа-ресурса</summary>
        public const string MediaUrl = "media_url";
        /// <summary>Прямой URL изображения</summary>
        public const string MediaUrlImage = "media_url_image";
        /// <summary>Массив URL изображений разных размеров</summary>
        public const string MediaUrlImages = "media_url_images";
        /// <summary>Оптимизированные URL изображений для отображения в ленте</summary>
        public const string MediaUrlImagesForFeed = "media_url_images_for_feed";
        /// <summary>Идентификаторы загруженных изображений</summary>
        public const string MediaUrlImageIds = "media_url_image_ids";
        /// <summary>Превью изображения, оптимизированное для групп</summary>
        public const string MediaUrlImagePreviewGroup = "media_url_image_preview_group";
        /// <summary>Превью изображения, оптимизированное для пользователей</summary>
        public const string MediaUrlImagePreviewUser = "media_url_image_preview_user";
        /// <summary>Обложка пользователя в медиа-представлении</summary>
        public const string MediaUserCover = "media_user_cover";
        /// <summary>Метаданные пользователя-автора медиа</summary>
        public const string MediaUserData = "media_user_data";
        /// <summary>Ссылки на видеоальбомы или подборки</summary>
        public const string MediaVideoAlbumRefs = "media_video_album_refs";
        /// <summary>Идентификатор настроения или эмодзи-статуса</summary>
        public const string MoodId = "mood_id";
        /// <summary>Конфигурация мотивационного или gamification-блока</summary>
        public const string MotivatorConfig = "motivator_config";
        /// <summary>Данные ссылки мотивационного элемента</summary>
        public const string MotivatorLinkData = "motivator_link_data";
        /// <summary>Ссылка на мотивационный контент или акцию</summary>
        public const string MotivatorRef = "motivator_ref";
        /// <summary>Причина, по которой пост нельзя редактировать</summary>
        public const string NotEditableReason = "not_editable_reason";
        /// <summary>Данные о коммерческом предложении или скидке</summary>
        public const string Offer = "offer";
        /// <summary>Находится ли запись на модерации</summary>
        public const string OnModeration = "on_moderation";
        /// <summary>Информация о заказе, если пост связан с покупкой</summary>
        public const string OrdInfo = "ord_info";
        /// <summary>Ссылка на владельца или правообладателя записи</summary>
        public const string OwnerRef = "owner_ref";
        /// <summary>Партнёрская или реферальная ссылка</summary>
        public const string PartnerLink = "partner_link";
        /// <summary>Разрешено ли закрепление поста в ленте</summary>
        public const string PinAllowed = "pin_allowed";
        /// <summary>Ссылка на геолокацию или привязанное место</summary>
        public const string PlaceRef = "place_ref";
        /// <summary>Данные о формате подачи или презентации контента</summary>
        public const string Presentation = "presentation";
        /// <summary>Статус редактирования товарной карточки</summary>
        public const string ProductEditStatus = "product_edit_status";
        /// <summary>Параметры сортировки товаров внутри поста</summary>
        public const string ProductOrdering = "product_ordering";
        /// <summary>Разрешено ли написание сообщения продавцу</summary>
        public const string ProductWriteMessage = "product_write_message";
        /// <summary>Текущий статус промо-кампании</summary>
        public const string PromoStatus = "promo_status";
        /// <summary>Количество дней публикации как бесплатного объявления</summary>
        public const string PublicationAsFreeDays = "publication_as_free_days";
        /// <summary>Дата публикации записи (timestamp в миллисекундах)</summary>
        public const string PublicationDateMs = "publication_date_ms";
        /// <summary>Темы для возражений или ответных обсуждений</summary>
        public const string RebuttalTopics = "rebuttal_topics";
        /// <summary>Реферер или источник перехода к записи</summary>
        public const string Ref = "ref";
        /// <summary>Количество репостов записи</summary>
        public const string ReshareCount = "reshare_count";
        /// <summary>Сводная информация по репостам и виральности</summary>
        public const string ReshareSummary = "reshare_summary";
        /// <summary>Время жизни статуса или кэш-валидность данных</summary>
        public const string StatusTtl = "status_ttl";
        /// <summary>Хештеги или тематические теги поста</summary>
        public const string Tags = "tags";
        /// <summary>Разрешено ли одобрение или отклонение темы обсуждения</summary>
        public const string TopicAcceptRejectAllowed = "topic_accept_reject_allowed";
        /// <summary>Разрешены ли комментарии в привязанной теме</summary>
        public const string TopicCommentAllowed = "topic_comment_allowed";
        /// <summary>Разрешена ли публикация в теме</summary>
        public const string TopicPublishAllowed = "topic_publish_allowed";
        /// <summary>Разрешено ли переключение веток комментариев</summary>
        public const string TopicSwitchCommentsAllowed = "topic_switch_comments_allowed";
        /// <summary>Разрешено ли изменение статуса записи</summary>
        public const string ToStatusAllowed = "to_status_allowed";
        /// <summary>Событие в профиле пользователя, связанное с постом</summary>
        public const string UserProfileEvent = "user_profile_event";
        /// <summary>Количество просмотров записи</summary>
        public const string ViewsCount = "views_count";
        /// <summary>Уровень видимости поста (public, friends, private и т.д.)</summary>
        public const string Visibility = "visibility";
        /// <summary>Данные ссылки на VK Clip (кроссплатформенная интеграция)</summary>
        public const string VkClipLinkData = "vk_clip_link_data";
        /// <summary>Теги виджета или встраиваемого элемента</summary>
        public const string WidgetTags = "widget_tags";
        /// <summary>Ссылки на друзей, отметивших или участвующих в посте</summary>
        public const string WithFriendRefs = "with_friend_refs";
}