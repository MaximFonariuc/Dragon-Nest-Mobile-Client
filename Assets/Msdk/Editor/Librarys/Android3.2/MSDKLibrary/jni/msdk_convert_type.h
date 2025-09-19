#ifndef SHAREREPO_FOUNDATION_MSDKCONVERT_MSDK_STRING_H_
#define SHAREREPO_FOUNDATION_MSDKCONVERT_MSDK_STRING_H_

#include <string.h>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>
#include "MSDKEnums.h"
#include "MSDKStructs.h"
#include <stdlib.h>

// 分配内存和释放内存
#define SAFE_MALLOC(num, type) (type *) calloc(num, sizeof(type))
#define SAFE_FREE(ptr)   \
do                   \
{                    \
    if (ptr != NULL) \
    {                \
        free(ptr);   \
        ptr = NULL;  \
    }                \
} while (0)

// 重新定义MSDKString
class MSDKString
{
   public:
    MSDKString(const std::string &str) : data(), length(0)
    {
        length = str.size();
        data = SAFE_MALLOC(length + 1, char);
        strcpy(data, str.c_str());
        data[length] = '\0';
    }
    MSDKString(const char *str_ptr = NULL) : data(), length(0)
    {
        if (str_ptr == NULL)
        {
            data = SAFE_MALLOC(1, char);
            data[0] = '\0';
            return;
        }

        length = strlen(str_ptr);
        data = SAFE_MALLOC(length + 1, char);
        strcpy(data, str_ptr);
        data[length] = '\0';
    }
    MSDKString(const MSDKString &str) : data(), length(0)
    {
        length = str.size();
        data = SAFE_MALLOC(length + 1, char);
        strcpy(data, str.c_str());
        data[length] = '\0';
    }
    // 析构函数
    ~MSDKString()
    {
        SAFE_FREE(data);
        length = 0;
    }
    // 重载+
    MSDKString operator+(const MSDKString &str) const
    {
        MSDKString new_str;
        new_str.length = length + str.size();
        new_str.data = SAFE_MALLOC(new_str.length + 1, char);
        strcpy(new_str.data, data);
        strcat(new_str.data, str.data);
        data[new_str.length] = '\0';

        return new_str;
    }
    // 重载=
    MSDKString &operator=(const MSDKString &str)
    {
        if (this == &str)
        {
            return *this;
        }

        SAFE_FREE(data);
        length = str.length;
        data = SAFE_MALLOC(length + 1, char);
        strcpy(data, str.c_str());
        data[length] = '\0';

        return *this;
    }
    // 重载+=
    MSDKString &operator+=(const MSDKString &str)
    {
        length += str.length;
        char *new_data = SAFE_MALLOC(length + 1, char);
        strcpy(new_data, data);
        strcat(new_data, str.data);
        SAFE_FREE(data);
        data = new_data;
        data[length] = '\0';

        return *this;
    }
    // 重载==
    inline bool operator==(const MSDKString &str) const
    {
        if (length != str.length)
        {
            return false;
        }
        return strcmp(data, str.data) ? false : true;
    }
    // 获取长度
    inline size_t size() const
    {
        return length;
    }
    const std::string ToString() const
    {
        if (data)
        {
            return data;
        }

        return "";
    }
    bool Empty()
    {
        return size() > 0 ? false : true;
    }
    // 获取C字符串
    inline const char *c_str() const
    {
        return data;
    }

   private:
    char *data;
    long length;
};

// 重新定义vector
template <typename T, int SPARE_CAPACITY = 16> class MSDKVector
{
   public:
    //将构造函数声明为explicit ,是为了抑制由构造函数定义的隐式转换
    explicit MSDKVector(unsigned long init_size = 0) : the_size_(0), the_capacity_(init_size + SPARE_CAPACITY), objects_(NULL)
    {
        objects_ = SAFE_MALLOC(init_size + SPARE_CAPACITY, T);
    }
    MSDKVector(const MSDKVector &rhs) : objects_(NULL)
    {
        the_size_ = rhs.the_size_;
        the_capacity_ = rhs.the_capacity_;

        objects_ = SAFE_MALLOC(the_capacity_, T);
        for (int k = 0; k < the_size_; k++)
        {
            objects_[k] = rhs.objects_[k];
        }
    }
    ~MSDKVector()
    {
        for (unsigned int i = 0; i < the_size_; i++)
        {
            objects_[i].~T();
        }
        SAFE_FREE(objects_);
    }
    const MSDKVector &operator=(const MSDKVector &rhs)
    {
        if (this != &rhs)
        {
            for (unsigned int i = 0; i < the_size_; i++)
            {
                objects_[i].~T();
            }
            SAFE_FREE(objects_);
            the_size_ = rhs.the_size_;
            the_capacity_ = rhs.the_capacity_;

            objects_ = SAFE_MALLOC(the_capacity_, T);
            for (int k = 0; k < the_size_; k++)
            {
                objects_[k] = rhs.objects_[k];
            }
        }

        return *this;
    }
    // 如果index错误，直接返回0的数据
    T &operator[](int index)
    {
        if (index < 0 || index >= the_size_)
        {
            return objects_[0];
        }
        return objects_[index];
    }
    const T &operator[](int index) const
    {
        return objects_[index];
    }
    //检测是否需要扩容
    void reserve()
    {
        reserve(the_size_);
    }
    // 扩容数据
    void reserve(int new_size)
    {
        if (the_capacity_ > new_size)
        {
            return;
        }
        int new_capacity = the_capacity_ * 2 + 1;
        T *old_arr = objects_;

        objects_ = SAFE_MALLOC(new_capacity, T);
        for (int k = 0; k < the_size_; k++)
        {
            objects_[k] = old_arr[k];
        }
        the_capacity_ = new_capacity;

        for (unsigned int i = 0; i < the_size_; i++)
        {
            old_arr[i].~T();
        }
        SAFE_FREE(old_arr);  // 删除原来的数据
    }
    int size() const
    {
        return the_size_;
    }
    int capacity() const
    {
        return the_capacity_;
    }
    bool Empty() const
    {
        return the_size_ == 0;
    }
    void resize(int new_size)
    {
        reserve(new_size);
        the_size_ = new_size;
        the_capacity_ = new_size;
    }
    void push_back(const T &obj)
    {
        reserve();  // 检测容器大小
        objects_[the_size_++] = obj;
    }
    void pop_back()
    {
        objects_[the_size_].~T();
        the_size_--;
    }
    const T &back() const
    {
        return objects_[the_size_ - 1];
    }
    T *begin()
    {
        return &objects_[0];
    }
    T *end()
    {
        return &objects_[the_size_];
    }
    const T *end() const
    {
        return &objects_[the_size_];
    }
    std::string ToString()
    {
//        std::stringstream out;
//        out << "Vecot长度:" << size() << ",容量：" << capacity() << std::endl;
//        for (int i = 0; i < the_size_; i++)
//        {
//            out << "objects[" << i << "]:" << objects_[i] << std::endl;
//        }
//
//        return out.str();
        return "";
    }

    typedef T *iterator;
    typedef const T *const_iterator;

   private:
    int the_size_;
    int the_capacity_;
    T *objects_;
};

/** ==================================================== **/
/** MSDK的内部转换数据的类型 **/
class MSDKKVPair
{
   private:
    MSDKString key;
    MSDKString value;

   public:
    MSDKKVPair()
    { }
    MSDKKVPair(std::string &_key, std::string &_value)
    {
        key = _key;
        value = _value;
    }
    KVPair ToKVPair()
    {
        KVPair tmp;
        tmp.key = key.c_str();
        tmp.value = value.c_str();

        return tmp;
    }
    // 转换为字符串
    std::string ToString()
    {
//        std::stringstream out;
//        out << "key:" << key.c_str() << ",value:" << value.c_str();
//
//        return out.str();
        return "";
    }
};
class MSDKPicInfo
{
    // 私有部分数据直接同步
private:
    eMSDK_SCREENDIR screenDir;  // 0：横竖屏   1：竖屏 2：横屏
    MSDKString picPath;  //图片本地路径
    //    ePicType type;         //图片类型
    MSDKString hashValue;  //图片hash值
public:
    MSDKPicInfo()
    { }
    MSDKPicInfo(eMSDK_SCREENDIR &_screenDir, std::string &_picPath,
                         std::string &_hashValue)
    {
        screenDir = _screenDir;
        picPath = _picPath;
        hashValue = _hashValue;
    }
    PicInfo ToPicInfo()
    {
        PicInfo tmp;
        tmp.screenDir = screenDir;
        tmp.picPath = picPath.c_str();
        tmp.hashValue = hashValue.c_str();

        return tmp;
    }
    // 转换为字符串
    std::string ToString()
    {
//        std::stringstream out;
//        out << "screenDir:" << screenDir << ",picPath:" << picPath.c_str() << ",hashValue:" << hashValue.c_str();
//
//        return out.str();
        return "";
    }
};
class MSDKNoticeInfo
{
// 私有部分数据直接同步
private:
    MSDKString msg_id;              //公告id
    MSDKString open_id;             //用户open_id
    MSDKString msg_url;             //公告跳转链接
    eMSG_NOTICETYPE msg_type;       //公告类型，eMSG_NOTICETYPE
    MSDKString msg_scene;           //公告展示的场景，管理端后台配置
    MSDKString start_time;          //公告有效期开始时间
    MSDKString end_time;            //公告有效期结束时间
    eMSG_CONTENTTYPE content_type;  //公告内容类型，eMSG_CONTENTTYPE
    //网页公告特殊字段
    MSDKString content_url;  //网页公告URL
    //图片公告特殊字段
    MSDKVector<MSDKPicInfo> picArray;  //图片数组
    //文本公告特殊字段
    MSDKString msg_title;    //公告标题
    MSDKString msg_content;  //公告内容
    MSDKString msg_custom; //自定义参数
#ifdef __APPLE__
    int msg_order;  //公告优先级，越大优先级越高，MSDK2.8.0版本新增
#endif

#ifdef ANDROID
    MSDKString msg_order;  //优先级，数字越大，优先级越高
#endif

public:
    MSDKNoticeInfo()
    { }
    MSDKNoticeInfo(std::string &_msg_id, std::string &_open_id,
                            std::string &_msg_url, eMSG_NOTICETYPE &_msg_type,
        std::string &_msg_scene, std::string &_start_time, std::string &_end_time, eMSG_CONTENTTYPE &_content_type,
        std::string &_content_url, std::vector<PicInfo> &_picArray, std::string &_msg_title, std::string &_msg_content,
        std::string &_msg_custom,
#ifdef __APPLE__
        int &_msg_order,
#endif
#ifdef ANDROID
        std::string &_msg_order,
#endif
        int none_data = -1)
    {
        msg_id = _msg_id;              //公告id
        open_id = _open_id;            //用户open_id
        msg_url = _msg_url;            //公告跳转链接
        msg_type = _msg_type;          //公告类型，eMSG_NOTICETYPE
        msg_scene = _msg_scene;        //公告展示的场景，管理端后台配置
        start_time = _start_time;      //公告有效期开始时间
        end_time = _end_time;          //公告有效期结束时间
        content_type = _content_type;  //公告内容类型，eMSG_CONTENTTYPE
        // 网页公告特殊字段
        content_url = _content_url;  //网页公告URL
        // 循环获取数据数据
        for (int i = 0; i < _picArray.size(); i++)
        {
            MSDKPicInfo tmp(_picArray[i].screenDir, _picArray[i].picPath, _picArray[i].hashValue);
            picArray.push_back(tmp);  // 拷贝数据
        }
        // 文本公告特殊字段
        msg_title = _msg_title;      //公告标题
        msg_content = _msg_content;  //公告内容
        msg_custom = _msg_custom; //自定义参数

#ifdef __APPLE__
        msg_order = _msg_order;  //公告优先级，越大优先级越高，MSDK2.8.0版本新增
#endif
#ifdef ANDROID
        msg_order = _msg_order;  //优先级，数字越大，优先级越高
#endif
    }
    // 获取notice数据
    NoticeInfo ToNoticeInfo()
    {
        NoticeInfo tmp;

        tmp.msg_id = msg_id.c_str();          //公告id
        tmp.open_id = open_id.c_str();        //用户open_id
        tmp.msg_url = msg_url.c_str();        //公告跳转链接
        tmp.msg_type = msg_type;              //公告类型，eMSG_NOTICETYPE
        tmp.msg_scene = msg_scene.c_str();    //公告展示的场景，管理端后台配置
        tmp.start_time = start_time.c_str();  //公告有效期开始时间
        tmp.end_time = end_time.c_str();      //公告有效期结束时间
        tmp.content_type = content_type;      //公告内容类型，eMSG_CONTENTTYPE
        // 网页公告特殊字段
        tmp.content_url = content_url.c_str();  //网页公告URL
        // 循环获取数据数据
        for (int i = 0; i < picArray.size(); i++)
        {
            PicInfo picinfo_tmp = picArray[i].ToPicInfo();
            tmp.picArray.push_back(picinfo_tmp);  // 拷贝数据
        }
        // 文本公告特殊字段
        tmp.msg_title = msg_title.c_str();      //公告标题
        tmp.msg_content = msg_content.c_str();  //公告内容
        tmp.msg_custom = msg_custom.c_str();//自定义参数
#ifdef __APPLE__
        tmp.msg_order = msg_order;  //公告优先级，越大优先级越高，MSDK2.8.0版本新增
#endif
#ifdef ANDROID
        tmp.msg_order = msg_order.c_str();  //优先级，数字越大，优先级越高
#endif

        return tmp;
    }
    // 转换为字符串
    std::string ToString()
    {
//        std::stringstream out;
//        out << "msg_id:" << msg_id.c_str() << ",open_id:" << open_id.c_str() << ",msg_title:" << msg_title.c_str()
//            << ",msg_content:" << msg_content.c_str() << ",PicInfo size:" << picArray.size();
//
//        return out.str();
        return "";
    }
};

// 登录接口
class ConvertTokenRet
{
private:
    int type;
    MSDKString value;
    long long expiration;

public:
    ConvertTokenRet()
    { }
    ConvertTokenRet(int _type, std::string _value, long long _expiration)
    {
        type = _type;
        value = _value.c_str();
        expiration = _expiration;
    }
    // 转换为TokenRet
    TokenRet ToTokenRet()
    {
        TokenRet ret;
        ret.type = type;
        ret.value = value.c_str();
        ret.expiration = expiration;

        return ret;
    }
};
class ConvertLoginRet
{
private:
    int flag;         //返回标记，标识成功和失败类型
    MSDKString desc;  //返回描述
    int platform;     //当前登录的平台
    MSDKString open_id;
    MSDKVector<ConvertTokenRet> token;
    MSDKString user_id;  //用户ID，先保留，等待和微信协商
    MSDKString pf;
    MSDKString pf_key;
    MSDKString prajna_ext;
    MSDKString extra_json;  //业务需要的额外字段，如scope

public:
    explicit ConvertLoginRet(int _flag, std::string _desc, int _platform,
                          std::string _open_id, std::vector<TokenRet> _token,
        std::string _user_id, std::string _pf, std::string _pf_key, std::string _prajna_ext, std::string _extra_json)
    {
        flag = _flag;
        desc = _desc.c_str();
        platform = _platform;
        open_id = _open_id.c_str();
        for (int i = 0; i < _token.size(); ++i)
        {
            ConvertTokenRet ret(_token[i].type, _token[i].value, _token[i].expiration);
            token.push_back(ret);
        }
        user_id = _user_id.c_str();
        pf = _pf.c_str();
        pf_key = _pf_key.c_str();
        prajna_ext = _prajna_ext.c_str();
        extra_json = _extra_json.c_str();
    }
    // 转换为LoginRet
    LoginRet ToLoginRet()
    {
        LoginRet ret;
        ret.flag = flag;
        ret.desc = desc.c_str();
        ret.platform = platform;
        ret.open_id = open_id.c_str();
        for (int i = 0; i < token.size(); ++i)
        {
            TokenRet tmp_tokenret = token[i].ToTokenRet();
            ret.token.push_back(tmp_tokenret);
        }
        ret.user_id = user_id.c_str();
        ret.pf = pf.c_str();
        ret.pf_key = pf_key.c_str();
        ret.prajna_ext = prajna_ext.c_str();
        ret.extra_json = extra_json.c_str();

        return ret;
    }
};
class ConvertQQGroup
{
private:
    MSDKString groupId;
    MSDKString groupName;
public:
    ConvertQQGroup()
    {}
    ConvertQQGroup(QQGroup &_qqGroup)
    {
        groupId = _qqGroup.groupId;
        groupName = _qqGroup.groupName;
    }
    QQGroup ToQQGroup()
    {
        QQGroup tmp;
        tmp.groupId = groupId.c_str();
        tmp.groupName = groupName.c_str();
        return tmp;
    }
};
class ConvertPersonInfo
{
private:
    MSDKString nickName;       //昵称
    MSDKString openId;         //帐号唯一标示
    MSDKString gender;         //性别
    MSDKString pictureSmall;   //小头像
    MSDKString pictureMiddle;  //中头像
    MSDKString pictureLarge;   // datouxiang
    MSDKString provice;        //省份(老版本属性，为了不让外部app改代码，没有放在AddressInfo)
    MSDKString city;           //城市(老版本属性，为了不让外部app改代码，没有放在AddressInfo)
    bool isFriend;              //是否好友
    int distance;               //离此次定位地点的距离
    MSDKString lang;           //语言
    MSDKString country;        //国家
    MSDKString gpsCity;        //根据GPS信息获取到的城市
    MSDKString unionID;        //用户unionID
    MSDKString friendRemark;    //微信好友备注，微信登录时若权限完整且后台返回备注时才存在

public:
    ConvertPersonInfo()
    { }
    ConvertPersonInfo(PersonInfo& personInfo)
    {
        nickName = personInfo.nickName.c_str();
        openId = personInfo.openId.c_str();
        gender = personInfo.gender.c_str();
        pictureSmall = personInfo.pictureSmall.c_str();
        pictureMiddle = personInfo.pictureMiddle.c_str();
        pictureLarge = personInfo.pictureLarge.c_str();
        provice = personInfo.provice.c_str();
        city = personInfo.city.c_str();
        isFriend = personInfo.isFriend;
        distance = personInfo.distance;
        lang = personInfo.lang.c_str();
        country = personInfo.country.c_str();
        gpsCity = personInfo.gpsCity.c_str();
        unionID = personInfo.unionID.c_str();
        friendRemark = personInfo.friendRemark.c_str();
    }
    PersonInfo ToPersonInfo()
    {
        PersonInfo tmp;
        tmp.nickName = nickName.c_str();
        tmp.openId = openId.c_str();
        tmp.gender = gender.c_str();
        tmp.pictureSmall = pictureSmall.c_str();
        tmp.pictureMiddle = pictureMiddle.c_str();
        tmp.pictureLarge = pictureLarge.c_str();
        tmp.provice = provice.c_str();
        tmp.city = city.c_str();
        tmp.isFriend = isFriend;
        tmp.distance = distance;
        tmp.lang = lang.c_str();
        tmp.country = country.c_str();
        tmp.gpsCity = gpsCity.c_str();
        tmp.unionID = unionID.c_str();
        tmp.friendRemark = friendRemark.c_str();

        return tmp;
    }
};

// 通用的转换静态函数
class MsdkCovertTypeUtils
{
public:
    // 从std::vector<NoticeInfo>转换为MSDKVector<MSDKNoticeInfo>
    static MSDKVector<MSDKNoticeInfo> VNoticeInfoToMSDKVNoticeInfo(std::vector<NoticeInfo> &v)
    {
        MSDKVector<MSDKNoticeInfo> tmp(v.size());
        for (int i = 0; i < v.size(); i++)
        {
            MSDKNoticeInfo msdknoticeinfo_tmp(v[i].msg_id, v[i].open_id, v[i].msg_url, v[i].msg_type, v[i].msg_scene,
                v[i].start_time, v[i].end_time, v[i].content_type, v[i].content_url, v[i].picArray, v[i].msg_title,
                v[i].msg_content,v[i].msg_custom,
#ifdef __APPLE__
                v[i].msg_order,
#endif
#ifdef ANDROID
                v[i].msg_order,
#endif
                -1);

            tmp.push_back(msdknoticeinfo_tmp);
        }

        return tmp;
    }
    // 从MSDKVector<MSDKNoticeInfo>转换为std::vector<NoticeInfo>
    static std::vector<NoticeInfo> MSDKVNoticeInfoToVNoticeInfo(MSDKVector<MSDKNoticeInfo> &v)
    {
        std::vector<NoticeInfo> tmp;
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i].ToNoticeInfo());
        }

        return tmp;
    }
    // 从std::vector<std::string>转换为MSDKVector<MSDKString>
    static MSDKVector<MSDKString> VStringToVMSDKString(std::vector<std::string> &v)
    {
        MSDKVector<MSDKString> tmp(v.size());
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i].c_str());
        }

        return tmp;
    }
    // 从MSDKVector<MSDKString>转换为std::vector<std::string>
    static std::vector<std::string> VMSDKStringToVString(MSDKVector<MSDKString> &v)
    {
        std::vector<std::string> tmp;
        for (int i = 0; i < v.size(); i++)
        {
            std::string string_tmp(v[i].c_str());
            tmp.push_back(string_tmp);
        }

        return tmp;
    }
    // 从std::vector<KVPair>转换为MSDKVector<MSDKKVPair>
    static MSDKVector<MSDKKVPair> VKVPairToMSDKVKVPair(std::vector<KVPair> &v)
    {
        MSDKVector<MSDKKVPair> tmp(v.size());
        for (int i = 0; i < v.size(); i++)
        {
            MSDKKVPair msdkkvpair_tmp(v[i].key, v[i].value);
            tmp.push_back(msdkkvpair_tmp);
        }

        return tmp;
    }
    // 从MSDKVector<MSDKKVPair>转换为std::vector<KVPair>
    static std::vector<KVPair> MSDKVKVPairToVKVPair(MSDKVector<MSDKKVPair> &v)
    {
        std::vector<KVPair> tmp;
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i].ToKVPair());
        }

        return tmp;
    }
    // 从std::vector<PersonInfo>转换为MSDKVector<ConvertPersonInfo>
    static MSDKVector<ConvertPersonInfo> VPersonInfoToMSDKVPersonInfo(
        std::vector<PersonInfo> &v)
    {
        MSDKVector<ConvertPersonInfo> tmp(v.size());
        for (int i = 0; i < v.size(); i++)
        {
            ConvertPersonInfo msdkperson_info_tmp = v[i];
            tmp.push_back(msdkperson_info_tmp);
        }

        return tmp;
    }
    // 从MSDKVector<MSDKKVPair>转换为std::vector<KVPair>
    static std::vector<PersonInfo> MSDKVPersonInfoToVPersonInfo(
        MSDKVector<ConvertPersonInfo> &v)
    {
        std::vector<PersonInfo> tmp;
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i].ToPersonInfo());
        }

        return tmp;
    }
    // 从std::vector<char>转换为MSDKVector<char>
    static MSDKVector<char> VCharToMSDKVChar(
        std::vector<char> &v)
    {
        MSDKVector<char> tmp(v.size());
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i]);
        }

        return tmp;
    }
    // 从MSDKVector<char>转换为std::vector<char>
    static std::vector<char> MSDKVCharToVChar(
        MSDKVector<char> &v)
    {
        std::vector<char> tmp;
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i]);
        }

        return tmp;
    }

    // 从std::vector<QQGroup>转换为MSDKVector<QQGroup>
    static MSDKVector<ConvertQQGroup> VQQGroupToMSDKVQQGroup(
        std::vector<QQGroup> &v)
    {
        MSDKVector<ConvertQQGroup> tmp(v.size());
        for (int i = 0; i < v.size(); i++)
        {
            ConvertQQGroup msdkqqgroup_info_tmp = v[i];
            tmp.push_back(msdkqqgroup_info_tmp);
        }
        return tmp;
    }
    // 从MSDKVector<QQGroup>转换为std::vector<QQGroup>
    static std::vector<QQGroup> MSDKVQQGroupToVQQGroup(
        MSDKVector<ConvertQQGroup> &v)
    {
        std::vector<QQGroup> tmp;
        for (int i = 0; i < v.size(); i++)
        {
            tmp.push_back(v[i].ToQQGroup());
        }

        return tmp;
    }

#ifndef __APPLE__
    // 从LoginRet转换为MSDKLoginRet
    static LoginRet MSDKLoginRetToLoginRet(ConvertLoginRet &ret)
    {
        return ret.ToLoginRet();
    }
    // 从MSDKLoginRet转换为LoginRet
    static ConvertLoginRet LoginRetToMSDKLoginRet(LoginRet &ret)
    {
        ConvertLoginRet msdkloginret(
            ret.flag, ret.desc, ret.platform, ret.open_id, ret.token, ret.user_id, ret.pf, ret.pf_key, ret.prajna_ext, ret.extra_json);

        return msdkloginret;
    }
#endif
};

// 实名制结构体
class ConvertRealNameAuthRet
{
private:
    int flag;          // 0成功
    int errorCode;     //平台返回参数，当flag非0时需关注
    MSDKString desc;  //错误信息
    int platform;      //平台
public:
    explicit ConvertRealNameAuthRet(int _flag,
                        int _errorCode,
                        std::string _desc,
                        int _platform)
    {
        flag = _flag;
        errorCode = _errorCode;
        desc = _desc;
        platform = _platform;
    }
    RealNameAuthRet ToRealNameAuthRet()
    {
        RealNameAuthRet tmp;
        tmp.flag = flag;
        tmp.errorCode = errorCode;
        tmp.desc = desc.c_str();
        tmp.platform = platform;

        return tmp;
    }
};
// 广告的结构体
class ConvertADRet
{
private:
    MSDKString viewTag;  // Button点击的tag
    _eADType scene;       //暂停位还是退出位

public:
    explicit ConvertADRet(std::string _viewTag, _eADType _scene)
    {
        viewTag = _viewTag;
        scene = _scene;
    }
    ADRet ToADRet()
    {
        ADRet tmp;
        tmp.viewTag = viewTag.c_str();
        tmp.scene = scene;

        return tmp;
    }
};
// 群结构体
class ConvertGroupRet
{
    class MSDKWXGroupInfo
    {
    private:
        MSDKString openIdList;   //群成员openId,以","分隔
        MSDKString memberNum;    //群成员数
        MSDKString chatRoomURL;  //创建（加入）群聊URL
        int status;               // 0表示没有创建或者加群，1表示已经创建群或者加群

    public:
        MSDKWXGroupInfo()
        { }
        MSDKWXGroupInfo(std::string _openIdList,
                        std::string _memberNum,
                        std::string _chatRoomURL,
                        int _status)
        {
            openIdList = _openIdList;
            memberNum = _memberNum;
            chatRoomURL = _chatRoomURL;
            status = _status;
        }
        MSDKWXGroupInfo(WXGroupInfo& tmp)
        {
            openIdList = tmp.openIdList;
            memberNum = tmp.memberNum;
            chatRoomURL = tmp.chatRoomURL;
            status = tmp.status;
        }
        WXGroupInfo ToWXGroupInfo()
        {
            WXGroupInfo tmp;
            tmp.openIdList = openIdList.c_str();
            tmp.memberNum = memberNum.c_str();
            tmp.chatRoomURL = chatRoomURL.c_str();
            tmp.status = status;

            return tmp;
        }
    };
    class MSDKQQGroupInfoV2
    {
    private:
        int relation;
        MSDKString guildId;
        MSDKString guildName;
        MSDKVector<ConvertQQGroup> qqGroups;

    public:
        MSDKQQGroupInfoV2()
        { }
        MSDKQQGroupInfoV2(int _relation,std::string _guildId,std::string _guildName,std::vector<QQGroup> _qqGroups)
        {
            relation = _relation;
            guildId = _guildId;
            guildName = _guildName;
            qqGroups = MsdkCovertTypeUtils::VQQGroupToMSDKVQQGroup(_qqGroups);
        }
        MSDKQQGroupInfoV2(QQGroupInfoV2 & tmp){
            relation = tmp.relation;
            guildId = tmp.guildId;
            guildName = tmp.guildName;
            qqGroups = MsdkCovertTypeUtils::VQQGroupToMSDKVQQGroup(tmp.qqGroups);
        }
        QQGroupInfoV2 ToQQGroupInfoV2()
        {
            QQGroupInfoV2 tmp;
            tmp.relation = relation;
            tmp.guildId = guildId.c_str();
            tmp.guildName = guildName.c_str();
            tmp.qqGroups = MsdkCovertTypeUtils::MSDKVQQGroupToVQQGroup(qqGroups);
            return tmp;
        }
    };

    class MSDKQQGroupInfo
    {
    private:
        MSDKString groupName;    //群名称
        MSDKString fingerMemo;   //群的相关简介
        MSDKString memberNum;    //群成员数
        MSDKString maxNum;       //该群可容纳的最多成员数
        MSDKString ownerOpenid;  //群主openid
        MSDKString unionid;      //与该QQ群绑定的公会ID
        MSDKString zoneid;       //大区ID
        MSDKString adminOpenids;  //管理员openid。如果管理员有多个的话，用“,”隔开，例如0000000000000000000000002329FBEF,0000000000000000000000002329FAFF
        //群openID
        MSDKString groupOpenid;  //和游戏公会ID绑定的QQ群的groupOpenid
        //加群用的群key
        MSDKString groupKey;  //需要添加的QQ群对应的key
        MSDKString relation;  //用户与群的关系,1群主，2管理员，3普通成员，4非成员

    public:
        MSDKQQGroupInfo()
        { }
        MSDKQQGroupInfo(std::string _groupName,
                        std::string _fingerMemo,
                        std::string _memberNum,
                        std::string _maxNum,
                        std::string _ownerOpenid,
                        std::string _unionid,
                        std::string _zoneid,
                        std::string _adminOpenids,
                        std::string _groupOpenid,
                        std::string _groupKey,
                        std::string _relation)
        {
            groupName = _groupName;
            fingerMemo = _fingerMemo;
            memberNum = _memberNum;
            maxNum = _maxNum;
            ownerOpenid = _ownerOpenid;
            unionid = _unionid;
            zoneid = _zoneid;
            adminOpenids = _adminOpenids;
            groupOpenid = _groupOpenid;
            groupKey = _groupKey;
            relation = _relation;
        }
        MSDKQQGroupInfo(QQGroupInfo& tmp)
        {
            groupName = tmp.groupName;
            fingerMemo = tmp.fingerMemo;
            memberNum = tmp.memberNum;
            maxNum = tmp.maxNum;
            ownerOpenid = tmp.ownerOpenid;
            unionid = tmp.unionid;
            zoneid = tmp.zoneid;
            adminOpenids = tmp.adminOpenids;
            groupOpenid = tmp.groupOpenid;
            groupKey = tmp.groupKey;
            relation = tmp.relation;
        }
        QQGroupInfo ToQQGroupInfo()
        {
            QQGroupInfo tmp;
            tmp.groupName = groupName.c_str();
            tmp.fingerMemo = fingerMemo.c_str();
            tmp.memberNum = memberNum.c_str();
            tmp.maxNum = maxNum.c_str();
            tmp.ownerOpenid = ownerOpenid.c_str();
            tmp.unionid = unionid.c_str();
            tmp.zoneid = zoneid.c_str();
            tmp.adminOpenids = adminOpenids.c_str();
            tmp.groupOpenid = groupOpenid.c_str();
            tmp.groupKey = groupKey.c_str();
            tmp.relation = relation.c_str();

            return tmp;
        }
    };
private:
    int flag;          // 0成功
    int errorCode;     //平台返回参数，当flag非0时需关注
    MSDKString desc;  //错误信息
    int platform;      //平台
    MSDKQQGroupInfoV2  mQQGroupInfoV2;
#ifdef __APPLE__
    MSDKWXGroupInfo wxGroupInfo;  //微信群信息
    MSDKQQGroupInfo qqGroupInfo;  // QQ群信息
#endif

#ifdef ANDROID
    MSDKWXGroupInfo mWXGroupInfo;  //微信群信息
    MSDKQQGroupInfo mQQGroupInfo;  // QQ群信息
#endif

public:
    explicit ConvertGroupRet(int _flag,
                          int _errorCode,
                          std::string _desc,
                          int _platform)
    {
        flag = _flag;
        errorCode = _errorCode;
        desc = _desc;
        platform = _platform;
    }
    void SetGroupInfo(WXGroupInfo& wxgroupinfo, QQGroupInfo& qqgroupinfo,QQGroupInfoV2& qqgroupinfoV2)
    {
        mQQGroupInfoV2 = qqgroupinfoV2;
#ifdef __APPLE__
        wxGroupInfo = wxgroupinfo;  //微信群信息
        qqGroupInfo = qqgroupinfo;  // QQ群信息
#endif

#ifdef ANDROID
        mWXGroupInfo = wxgroupinfo;
        mQQGroupInfo = qqgroupinfo;  // QQ群信息
#endif
    }
    GroupRet ToGroupRet()
    {
        GroupRet tmp;
        tmp.flag = flag;
        tmp.errorCode = errorCode;
        tmp.desc = desc.c_str();
        tmp.platform = platform;
        tmp.mQQGroupInfoV2 = mQQGroupInfoV2.ToQQGroupInfoV2();
#ifdef __APPLE__
        tmp.wxGroupInfo = wxGroupInfo.ToWXGroupInfo();  //微信群信息
        tmp.qqGroupInfo = qqGroupInfo.ToQQGroupInfo();  // QQ群信息
#endif

#ifdef ANDROID
        tmp.mWXGroupInfo = mWXGroupInfo.ToWXGroupInfo(); //微信群信息
        tmp.mQQGroupInfo = mQQGroupInfo.ToQQGroupInfo(); // QQ群信息
#endif
        return tmp;
    }
};
// 浏览器
class ConvertWebviewRet
{
private:
    int flag;  // 0成功
    MSDKString msgData; //
public:
    explicit ConvertWebviewRet(int _flag)
    {
        flag = _flag;
    }
    ConvertWebviewRet(int _flag, std::string _msgdata){
        flag = _flag;
        msgData = _msgdata;
    }
    WebviewRet ToWebviewRet()
    {
        WebviewRet tmp;
        tmp.flag = flag;
        tmp.msgData = msgData.c_str();
        return tmp;
    }
};
// 分享结构体
class ConvertShareRet
{
private:
    int platform;         //平台类型
    int flag;             //操作结果
    MSDKString desc;     //结果描述（保留）
    MSDKString extInfo;  //游戏分享是传入的自定义字符串，用来标示分

public:
    explicit ConvertShareRet(int _platform,
                          int _flag,
                          std::string _desc,
                          std::string _extInfo)
    {
        platform = _platform;
        flag = _flag;
        desc = _desc;
        extInfo = _extInfo;
    }
    ShareRet ToShareRet()
    {
        ShareRet tmp;
        tmp.platform = platform;
        tmp.flag = flag;
        tmp.desc = desc.c_str();
        tmp.extInfo = extInfo.c_str();

        return tmp;
    }
};
class ConvertLocationRet
{
private:
    int flag;
    MSDKString desc;
    double longitude;
    double latitude;

public:
    explicit ConvertLocationRet(int _flag,
                             std::string _desc,
                             double _longitude,
                             double _latitude)
    {
        flag = _flag;
        desc = _desc;
        longitude = _longitude;
        latitude = _latitude;
    }
    LocationRet ToLocationRet()
    {
        LocationRet tmp;
        tmp.flag = flag;
        tmp.desc = desc.c_str();
        tmp.longitude = longitude;
        tmp.latitude = latitude;

        return tmp;
    }
};
class ConvertGetCountryFromIPRet
{
private:
    int ret;
    MSDKString msg;
    MSDKString country;
    bool is_query_by_request_header;

public:
    explicit ConvertGetCountryFromIPRet(int _ret,
                                        std::string _msg,
                                        std::string _country,
                                        bool _is_query_by_request_header)
    {
        ret = _ret;
        msg = _msg;
        country = _country;
        is_query_by_request_header = _is_query_by_request_header;
    }
    GetCountryFromIPRet ToGetCountryFromIPRet()
    {
        GetCountryFromIPRet tmp;
        tmp.ret = ret;
        tmp.msg = msg.c_str();
        tmp.country = country.c_str();
        tmp.isQueryByRequestHeader = is_query_by_request_header;
        return tmp;
    }
};
class ConvertGetIPInfoRet
{
private:
    int ret;
    int country_no;
    int province_no;
    int city_no;
    MSDKString msg;
    MSDKString country;
    MSDKString province;
    MSDKString city;
    bool is_query_by_request_header;
    
public:
    explicit ConvertGetIPInfoRet(int _ret,
                                 int _country_no,
                                 int _province_no,
                                 int _city_no,
                                 std::string _msg,
                                 std::string _country,
                                 std::string _province,
                                 std::string _city,
                                 bool _is_query_by_request_header)
    {
        ret = _ret;
        country_no = _country_no;
        province_no = _province_no;
        city_no = _city_no;
        msg = _msg;
        country = _country;
        province = _province;
        city = _city;
        is_query_by_request_header = _is_query_by_request_header;
    }
    GetIPInfoRet ToGetIPInfoRet()
    {
        GetIPInfoRet tmp;
        tmp.ret = ret;
        tmp.country_no = country_no;
        tmp.province_no = province_no;
        tmp.city_no = city_no;
        tmp.msg = msg.c_str();
        tmp.country = country.c_str();
        tmp.province = province.c_str();
        tmp.city = city.c_str();
        tmp.isQueryByRequestHeader = is_query_by_request_header;
        return tmp;
    }
};
class ConvertWakeupRet
{
private:
    int flag;                     //错误码
    int platform;                 //被什么平台唤起
    MSDKString media_tag_name;   // wx回传得meidaTagName
    MSDKString open_id;          // qq传递的openid
    MSDKString desc;             //描述
    MSDKString lang;             //语言     目前只有微信5.1以上用，手Q不用
    MSDKString country;          //国家     目前只有微信5.1以上用，手Q不用
    MSDKString messageExt;       //游戏分享传入自定义字符串，平台拉起游戏不做任何处理返回
    //目前只有微信5.1以上用，手Q不用
    MSDKVector<MSDKKVPair> extInfo;  //游戏－平台携带的自定义参数手Q专用
public:
    ConvertWakeupRet(int _flag,
                  int _platform,
                  std::string _media_tag_name,
                  std::string _open_id,
                  std::string _desc,
                  std::string _lang,
                  std::string _country,
                  std::string _messageExt,
                  std::vector<KVPair> _extInfo
    )
    {
        flag = _flag;
        platform = _platform;
        media_tag_name = _media_tag_name.c_str();
        open_id = _open_id.c_str();
        desc = _desc.c_str();
        lang = _lang.c_str();
        country = _country.c_str();
        messageExt = _messageExt.c_str();
        extInfo = MsdkCovertTypeUtils::VKVPairToMSDKVKVPair(_extInfo);
    }
    WakeupRet ToWakeupRet()
    {
        WakeupRet tmp;
        tmp.flag = flag;
        tmp.platform = platform;
        tmp.media_tag_name = media_tag_name.c_str();
        tmp.open_id = open_id.c_str();
        tmp.desc = desc.c_str();
        tmp.lang = lang.c_str();
        tmp.country = country.c_str();
        tmp.messageExt = messageExt.c_str();
        tmp.extInfo = MsdkCovertTypeUtils::MSDKVKVPairToVKVPair(extInfo);

        return tmp;
    }
};

class ConvertRelationRet
{
private:
    int flag;                         //查询结果flag，0为成功
    MSDKString desc;                 // 描述
    MSDKVector<ConvertPersonInfo> persons;  //保存好友或个人信息
    MSDKString extInfo;              //游戏查询是传入的自定义字段，用来标示一次查询
    eRelationRetType type;             //区分回调来源 0为个人信息回调 1为同玩好友回调 3为unionid回调
public:
    ConvertRelationRet(int _flag,
                    std::string _desc,
                    std::vector<PersonInfo> _persons,
                    std::string _extInfo,
                    eRelationRetType _type
    )
    {
        flag = _flag;
        desc = _desc;
        persons = MsdkCovertTypeUtils::VPersonInfoToMSDKVPersonInfo(_persons);
        extInfo = _extInfo.c_str();
        type = _type;
    }
    RelationRet ToRelationRet()
    {
        RelationRet tmp;
        tmp.flag = flag;
        tmp.desc = desc.c_str();
        tmp.persons = MsdkCovertTypeUtils::MSDKVPersonInfoToVPersonInfo
            (persons);
        tmp.extInfo = extInfo.c_str();
        tmp.type = type;

        return tmp;
    }
};

#ifdef ANDROID
class ConvertCardRet
{
private:
    int platform;                 //平台类型
    int flag;                     //操作结果
    MSDKString desc;             //结果描述（保留）
    MSDKString open_id;          // qq传递的openid
    MSDKString wx_card_list;     // card信息
    MSDKVector<MSDKKVPair> extInfo;  //游戏－平台携带的自定义参数手Q专用

public:
    ConvertCardRet(CardRet &cardret)
    {
        platform = cardret.platform;
        flag = cardret.flag;
        desc = cardret.desc.c_str();
        open_id = cardret.open_id.c_str();
        wx_card_list = cardret.wx_card_list.c_str();
        extInfo = MsdkCovertTypeUtils::VKVPairToMSDKVKVPair(cardret.extInfo);
    }
    CardRet ToCardRet()
    {
        CardRet tmp;
        tmp.platform = platform;
        tmp.flag = flag;
        tmp.desc = desc.c_str();
        tmp.open_id = open_id.c_str();
        tmp.wx_card_list = wx_card_list.c_str();
        tmp.extInfo = MsdkCovertTypeUtils::MSDKVKVPairToVKVPair(extInfo);

        return tmp;
    }
};
#endif

#endif //SHAREREPO_FOUNDATION_MSDKCONVERT_MSDK_STRING_H_
