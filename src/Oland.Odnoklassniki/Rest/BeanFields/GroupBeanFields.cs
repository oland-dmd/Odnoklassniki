namespace Oland.Odnoklassniki.Rest.BeanFields;

/// <summary>
/// Константы полей группы для параметра <c>fields</c> в методах groups.*
/// </summary>
public static class GroupBeanFields
{
    /// <summary>Аббревиатура или короткое название группы</summary>
    public const string Abbreviation = "abbreviation";
        
    /// <summary>Тип доступа к группе (открытая, закрытая, частная)</summary>
    public const string AccessType = "access_type";
        
    /// <summary>Юридический или фактический адрес группы</summary>
    public const string Address = "address";
        
    /// <summary>Разрешено ли добавление каналов в группу</summary>
    public const string AddChannelAllowed = "add_channel_allowed";
        
    /// <summary>Разрешено ли создание платных тем</summary>
    public const string AddPaidThemeAllowed = "add_paid_theme_allowed";
        
    /// <summary>Разрешено ли добавление фотоальбомов</summary>
    public const string AddPhotoAlbumAllowed = "add_photoalbum_allowed";
        
    /// <summary>Разрешено ли создание тем обсуждений</summary>
    public const string AddThemeAllowed = "add_theme_allowed";
        
    /// <summary>Разрешено ли добавление видео</summary>
    public const string AddVideoAllowed = "add_video_allowed";
        
    /// <summary>Идентификатор администратора группы</summary>
    public const string AdminId = "admin_id";
        
    /// <summary>Разрешено ли управление рекламой через кабинет</summary>
    public const string AdsManagerAllowed = "ads_manager_allowed";
        
    /// <summary>Разрешены ли расширенные публикации</summary>
    public const string AdvancedPublicationAllowed = "advanced_publication_allowed";
        
    /// <summary>Возрастное ограничение группы</summary>
    public const string AgeRestricted = "age_restricted";
        
    /// <summary>Статус авторского подтверждения контента</summary>
    public const string AuthorApproved = "author_approved";
        
    /// <summary>Источник автоматического наполнения группы</summary>
    public const string AutoGroupSource = "auto_group_source";
        
    /// <summary>Заблокирована ли группа</summary>
    public const string Blocked = "blocked";
        
    /// <summary>Добавлена ли группа в закладки текущим пользователем</summary>
    public const string Bookmarked = "bookmarked";
        
    /// <summary>Является ли группа бизнес-профилем</summary>
    public const string Business = "business";
        
    /// <summary>Разрешены ли звонки через группу</summary>
    public const string CallAllowed = "call_allowed";
        
    /// <summary>Разрешено ли управление монетизацией видео</summary>
    public const string CanManageVideoRevenue = "can_manage_video_revenue";
        
    /// <summary>Разрешено ли создание каталога товаров</summary>
    public const string CatalogCreateAllowed = "catalog_create_allowed";
        
    /// <summary>Основная категория группы</summary>
    public const string Category = "category";
        
    /// <summary>Разрешено ли изменение аватара</summary>
    public const string ChangeAvatarAllowed = "change_avatar_allowed";
        
    /// <summary>Разрешено ли изменение типа группы</summary>
    public const string ChangeTypeAllowed = "change_type_allowed";
        
    /// <summary>Город привязки группы</summary>
    public const string City = "city";
        
    /// <summary>Возможность комментировать от имени официальной страницы</summary>
    public const string CommentAsOfficial = "comment_as_official";
        
    /// <summary>Признак сообщества (true/false)</summary>
    public const string Community = "community";
        
    /// <summary>Возможность публиковать контент от имени официальной страницы</summary>
    public const string ContentAsOfficial = "content_as_official";
        
    /// <summary>Страна привязки группы</summary>
    public const string Country = "country";
        
    /// <summary>Информация об обложке группы</summary>
    public const string Cover = "cover";
        
    /// <summary>Кнопки на обложке группы</summary>
    public const string CoverButtons = "cover_buttons";
        
    /// <summary>Серия обложек (для ротации)</summary>
    public const string CoverSeries = "cover_series";
        
    /// <summary>Дата создания группы в миллисекундах (epoch)</summary>
    public const string CreatedMs = "created_ms";
        
    /// <summary>Разрешено ли создание рекламных объявлений</summary>
    public const string CreateAdsAllowed = "create_ads_allowed";
        
    /// <summary>Разрешены ли ежедневные фотопосты</summary>
    public const string DailyPhotoPostAllowed = "daily_photo_post_allowed";
        
    /// <summary>Разрешено ли удаление группы</summary>
    public const string DeleteAllowed = "delete_allowed";
        
    /// <summary>Описание группы</summary>
    public const string Description = "description";
        
    /// <summary>Отключена ли загрузка фотографий</summary>
    public const string DisablePhotoUpload = "disable_photo_upload";
        
    /// <summary>Причина отключения функционала</summary>
    public const string DisableReason = "disable_reason";
        
    /// <summary>Разрешено ли редактирование информации группы</summary>
    public const string EditAllowed = "edit_allowed";
        
    /// <summary>Разрешено ли управление приложениями</summary>
    public const string EditAppsAllowed = "edit_apps_allowed";
        
    /// <summary>Дата окончания акции или ограничения</summary>
    public const string EndDate = "end_date";
        
    /// <summary>Подписка на ленту новостей группы</summary>
    public const string FeedSubscription = "feed_subscription";
        
    /// <summary>Количество подписчиков</summary>
    public const string FollowersCount = "followers_count";
        
    /// <summary>Разрешено ли подписываться на группу</summary>
    public const string FollowAllowed = "follow_allowed";
        
    /// <summary>Список друзей в группе</summary>
    public const string Friends = "friends";
        
    /// <summary>Количество друзей участников</summary>
    public const string FriendsCount = "friends_count";
        
    /// <summary>Признак государственной организации</summary>
    public const string GosOrg = "gos_org";
        
    /// <summary>Год выпуска/окончания (для учебных групп)</summary>
    public const string GraduateYear = "graduate_year";
        
    /// <summary>Текст пользовательского соглашения группы</summary>
    public const string GroupAgreement = "group_agreement";
        
    /// <summary>Разрешено ли создание групповых челленджей</summary>
    public const string GroupChallengeCreateAllowed = "group_challenge_create_allowed";
        
    /// <summary>Разрешено ли ведение группового журнала</summary>
    public const string GroupJournalAllowed = "group_journal_allowed";
        
    /// <summary>Новостная лента группы</summary>
    public const string GroupNews = "group_news";
        
    /// <summary>Включён ли режим ежедневных фото</summary>
    public const string HasDailyPhoto = "has_daily_photo";
        
    /// <summary>Наличие пользовательского соглашения</summary>
    public const string HasGroupAgreement = "has_group_agreement";
        
    /// <summary>Наличие недавних подарков в группе</summary>
    public const string HasRecentPresents = "has_recent_presents";
        
    /// <summary>Экспертная тематика (хобби)</summary>
    public const string HobbyExpert = "hobby_expert";
        
    /// <summary>Тематика хобби/интересов</summary>
    public const string HobbyTopic = "hobby_topic";
        
    /// <summary>Название сайта/страницы</summary>
    public const string HomepageName = "homepage_name";
        
    /// <summary>URL внешнего сайта</summary>
    public const string HomepageUrl = "homepage_url";
        
    /// <summary>Количество приглашений</summary>
    public const string InvitationsCount = "invitations_count";
        
    /// <summary>Отправлено ли приглашение текущему пользователю</summary>
    public const string InvitationSent = "invitation_sent";
        
    /// <summary>Разрешено ли приглашать пользователей</summary>
    public const string InviteAllowed = "invite_allowed";
        
    /// <summary>Разрешено ли бесплатное приглашение</summary>
    public const string InviteFreeAllowed = "invite_free_allowed";
        
    /// <summary>Разрешено ли вступление в группу</summary>
    public const string JoinAllowed = "join_allowed";
        
    /// <summary>Количество заявок на вступление</summary>
    public const string JoinRequestsCount = "join_requests_count";
        
    /// <summary>Разрешён ли выход из группы</summary>
    public const string LeaveAllowed = "leave_allowed";
        
    /// <summary>Разрешено ли создание карусели ссылок</summary>
    public const string LinkCarouselAllowed = "link_carousel_allowed";
        
    /// <summary>Разрешён ли постинг ссылок</summary>
    public const string LinkPostingAllowed = "link_posting_allowed";
        
    /// <summary>Идентификатор локации</summary>
    public const string LocationId = "location_id";
        
    /// <summary>Широта геолокации</summary>
    public const string LocationLatitude = "location_latitude";
        
    /// <summary>Долгота геолокации</summary>
    public const string LocationLongitude = "location_longitude";
        
    /// <summary>Масштаб карты для отображения</summary>
    public const string LocationZoom = "location_zoom";
        
    /// <summary>Вкладка главной страницы по умолчанию</summary>
    public const string MainPageTab = "main_page_tab";
        
    /// <summary>Основная фотография группы</summary>
    public const string MainPhoto = "main_photo";
        
    /// <summary>Права управления участниками</summary>
    public const string ManageMembers = "manage_members";
        
    /// <summary>Разрешено ли управление сообщениями</summary>
    public const string ManageMessagingAllowed = "manage_messaging_allowed";
        
    /// <summary>Количество участников группы</summary>
    public const string MembersCount = "members_count";
        
    /// <summary>Статус участника в группе</summary>
    public const string MemberStatus = "member_status";
        
    /// <summary>Подписка на упоминания</summary>
    public const string MentionsSubscription = "mentions_subscription";
        
    /// <summary>Разрешена ли подписка на упоминания</summary>
    public const string MentionsSubscriptionAllowed = "mentions_subscription_allowed";
        
    /// <summary>Разрешены ли личные сообщения</summary>
    public const string MessagesAllowed = "messages_allowed";
        
    /// <summary>Разрешён ли мессенджер</summary>
    public const string MessagingAllowed = "messaging_allowed";
        
    /// <summary>Включён ли мессенджер</summary>
    public const string MessagingEnabled = "messaging_enabled";
        
    /// <summary>Минимальный возраст для вступления</summary>
    public const string MinAge = "min_age";
        
    /// <summary>Мобильная версия обложки</summary>
    public const string MobileCover = "mobile_cover";
        
    /// <summary>Название группы</summary>
    public const string Name = "name";
        
    /// <summary>Разрешены ли новые рекламные объявления</summary>
    public const string NewAdvertsAllowed = "new_adverts_allowed";
        
    /// <summary>Количество новых чатов</summary>
    public const string NewChatsCount = "new_chats_count";
        
    /// <summary>Подписка на уведомления</summary>
    public const string NotificationsSubscription = "notifications_subscription";
        
    /// <summary>Разрешены ли онлайн-платежи</summary>
    public const string OnlinePaymentAllowed = "online_payment_allowed";
        
    /// <summary>Платный доступ к группе</summary>
    public const string PaidAccess = "paid_access";
        
    /// <summary>Описание платного доступа</summary>
    public const string PaidAccessDescription = "paid_access_description";
        
    /// <summary>Стоимость платного доступа</summary>
    public const string PaidAccessPrice = "paid_access_price";
        
    /// <summary>Платный контент в группе</summary>
    public const string PaidContent = "paid_content";
        
    /// <summary>Описание платного контента</summary>
    public const string PaidContentDescription = "paid_content_description";
        
    /// <summary>Стоимость платного контента</summary>
    public const string PaidContentPrice = "paid_content_price";
        
    /// <summary>Разрешено ли создание партнёрских ссылок</summary>
    public const string PartnerLinkCreateAllowed = "partner_link_create_allowed";
        
    /// <summary>Разрешена ли партнёрская программа</summary>
    public const string PartnerProgramAllowed = "partner_program_allowed";
        
    /// <summary>Статус партнёрской программы</summary>
    public const string PartnerProgramStatus = "partner_program_status";
        
    /// <summary>Разрешены ли штрафные баллы</summary>
    public const string PenaltyPointsAllowed = "penalty_points_allowed";
        
    /// <summary>Контактный телефон</summary>
    public const string Phone = "phone";
        
    /// <summary>Скрыта ли вкладка фотографий</summary>
    public const string PhotosTabHidden = "photos_tab_hidden";
        
    /// <summary>Идентификатор основной фотографии</summary>
    public const string PhotoId = "photo_id";
        
    /// <summary>URL аватара</summary>
    public const string PicAvatar = "pic_avatar";
        
    /// <summary>Отключены ли PIN-уведомления</summary>
    public const string PinNotificationsOff = "pin_notifications_off";
        
    /// <summary>Лимит возможного количества участников</summary>
    public const string PossibleMembersCount = "possible_members_count";
        
    /// <summary>Статус премиум-подписки</summary>
    public const string Premium = "premium";
        
    /// <summary>Приватность группы</summary>
    public const string Private = "private";
        
    /// <summary>Скрыта ли вкладка товаров</summary>
    public const string ProductsTabHidden = "products_tab_hidden";
        
    /// <summary>Разрешено ли создание товаров</summary>
    public const string ProductCreateAllowed = "product_create_allowed";
        
    /// <summary>Разрешено ли предлагать товары</summary>
    public const string ProductCreateSuggestedAllowed = "product_create_suggested_allowed";
        
    /// <summary>Разрешено ли создание товаров с нулевым сроком жизни</summary>
    public const string ProductCreateZeroLifetimeAllowed = "product_create_zero_lifetime_allowed";
        
    /// <summary>Кнопки профиля</summary>
    public const string ProfileButtons = "profile_buttons";
        
    /// <summary>Разрешены ли промо-темы</summary>
    public const string PromoThemeAllowed = "promo_theme_allowed";
        
    /// <summary>Разрешена ли публикация отложенных тем</summary>
    public const string PublishDelayedThemeAllowed = "publish_delayed_theme_allowed";
        
    /// <summary>Реферер или источник перехода</summary>
    public const string Ref = "ref";
        
    /// <summary>Отправлена ли заявка на вступление</summary>
    public const string RequestSent = "request_sent";
        
    /// <summary>Дата отправки заявки</summary>
    public const string RequestSentDate = "request_sent_date";
        
    /// <summary>Разрешён ли репост</summary>
    public const string ReshareAllowed = "reshare_allowed";
        
    /// <summary>Включена ли партнёрская программа монетизации</summary>
    public const string RevenuePartnerProgramEnabled = "revenue_partner_program_enabled";
        
    /// <summary>Маркировка РКН</summary>
    public const string RknMark = "rkn_mark";
        
    /// <summary>Роль текущего пользователя в группе</summary>
    public const string Role = "role";
        
    /// <summary>Идентификатор области видимости</summary>
    public const string ScopeId = "scope_id";
        
    /// <summary>Видимость магазина для администраторов</summary>
    public const string ShopVisibleAdmin = "shop_visible_admin";
        
    /// <summary>Видимость магазина для публичного доступа</summary>
    public const string ShopVisiblePublic = "shop_visible_public";
        
    /// <summary>Короткое имя (slug)</summary>
    public const string Shortname = "shortname";
        
    /// <summary>Дата начала акции или ограничения</summary>
    public const string StartDate = "start_date";
        
    /// <summary>Разрешён ли просмотр статистики</summary>
    public const string StatsAllowed = "stats_allowed";
        
    /// <summary>Статус группы (активна, на модерации и т.д.)</summary>
    public const string Status = "status";
        
    /// <summary>Подкатегория группы</summary>
    public const string Subcategory = "subcategory";
        
    /// <summary>Идентификатор подкатегории</summary>
    public const string SubcategoryId = "subcategory_id";
        
    /// <summary>Разрешено ли предложение тем</summary>
    public const string SuggestThemeAllowed = "suggest_theme_allowed";
        
    /// <summary>Теги группы</summary>
    public const string Tags = "tags";
        
    /// <summary>Разрешены ли переводы средств</summary>
    public const string TransfersAllowed = "transfers_allowed";
        
    /// <summary>Уникальный идентификатор группы (старый формат)</summary>
    public const string Uid = "uid";
        
    /// <summary>Разрешена ли отписка</summary>
    public const string UnfollowAllowed = "unfollow_allowed";
        
    /// <summary>Платный доступ активен у пользователя</summary>
    public const string UserPaidAccess = "user_paid_access";
        
    /// <summary>Срок окончания платного доступа у пользователя</summary>
    public const string UserPaidAccessTill = "user_paid_access_till";
        
    /// <summary>Платный контент активен у пользователя</summary>
    public const string UserPaidContent = "user_paid_content";
        
    /// <summary>Срок окончания доступа к платному контенту</summary>
    public const string UserPaidContentTill = "user_paid_content_till";
        
    /// <summary>Скрыта ли вкладка видео</summary>
    public const string VideoTabHidden = "video_tab_hidden";
        
    /// <summary>Разрешён ли просмотр списка участников</summary>
    public const string ViewMembersAllowed = "view_members_allowed";
        
    /// <summary>Разрешён ли просмотр списка модераторов</summary>
    public const string ViewModeratorsAllowed = "view_moderators_allowed";
        
    /// <summary>Разрешён ли просмотр платных тем</summary>
    public const string ViewPaidThemesAllowed = "view_paid_themes_allowed";
        
    /// <summary>Разрешён ли просмотр полученных подарков</summary>
    public const string ViewReceivedPresentsAllowed = "view_received_presents_allowed";
        
    /// <summary>Разрешён ли просмотр отправленных подарков</summary>
    public const string ViewSentPresentsAllowed = "view_sent_presents_allowed";
        
    /// <summary>Кто может комментировать посты</summary>
    public const string WhoCanComment = "who_can_comment";
        
    /// <summary>Год начала деятельности</summary>
    public const string YearFrom = "year_from";
        
    /// <summary>Год окончания деятельности</summary>
    public const string YearTo = "year_to";
}