using System.Collections.Generic;
using System.IO;

namespace Cleavcats
{
    /// <summary>
    /// // 翻译系统
    /// <br>// 使用说明：先 Load All Space 读取 zh_cn，再读取目标翻译语言，将 language_default 置为 zh_cn，language_using 置为目标翻译语言</br>
    /// </summary>
    public class translate_Factory
    {
        /// <summary>
        /// // 原始数据集，language，space，book
        /// </summary>
        public SortedDictionary<string, SortedDictionary<string, CsvBook>> data = new SortedDictionary<string, SortedDictionary<string, CsvBook>>();
        /// <summary>
        /// // 设置此项来 指定默认语言，当 using 语言没有找到指定项时，尝试从此处得到
        /// </summary>
        public string language_default = null;
        /// <summary>
        /// // 优先使用此处的 翻译文本
        /// </summary>
        public string language_using = null;

        /// <summary>
        /// // 清除所有 已注入的 书
        /// </summary>
        public void Clear() { data.Clear(); }
        /// <summary>
        /// // 向库中增加一个 csv文件，如果已存在则覆盖之
        /// </summary>
        /// <param name="book_name"></param>
        /// <param name="book"></param>
        /// <param name="language_name"></param>
        public void SpaceAdd(string book_name, CsvBook book, string language_name)
        {
            SortedDictionary<string, CsvBook> founded;
        jump_to:
            if (data.TryGetValue(language_name, out founded))
            {
                // 覆盖书本
                if (founded.ContainsKey(book_name)) { founded[book_name] = book; }
                // 添加书本
                else founded.Add(book_name, book);
                return;
            }
            else
            {
                /*新的语言*/
                founded = new SortedDictionary<string, CsvBook>();
                data.Add(language_name, founded);
                goto jump_to;
            }
        }
        /// <summary>
        /// // 快速初始化，路径需要携带末尾的 /
        /// </summary>
        /// <param name="dir">// 需要携带末尾的 /</param>
        /// <param name="language_default"></param>
        public void QuickInit(string dir, string language_default)
        {
            this.LoadAllSpace(dir + language_default, language_default);
            this.language_default = language_default;
            this.language_using = language_default;
        }
        /// <summary>
        /// // 扫描目标文件夹下 的所有csv 文件（不包括子文件夹），全部作为 该 language 录入
        /// </summary>
        public void LoadAllSpace(string dir_path, string language)
        {
            DirectoryInfo dir_parent = new DirectoryInfo(dir_path);
            if (dir_parent.Exists == false) return;
            FileInfo[] dirs = dir_parent.GetFiles("*.csv");
            foreach (FileInfo file_path in dirs)
            {
                string book_name = Path.GetFileNameWithoutExtension(file_path.Name);
                Csv csv = Csv.ReadFromFile(file_path.FullName);
                CsvBook book = new CsvBook();
                book.ReadFromCsv(csv);
                this.SpaceAdd(book_name, book, language);
            }
        }

        /// <summary>
        /// // 取出文本（文件名+键名+下标），不存在则返回 Null
        /// <br>// 若 using 语言中不存在该条目，则从 default 语言中寻找</br>
        /// </summary>
        /// <param name="book_name"></param>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <param name="language_name"></param>
        /// <returns></returns>
        public string DataGet(string book_name, string key, int index = 0, string language_name = null)
        {
            if (language_name == null) language_name = this.language_using;
            SortedDictionary<string, CsvBook> founded;
            CsvBook founded_book;
            if (data.ContainsKey(language_name))
            {
                founded = data[language_name];
                if (founded.ContainsKey(book_name) == false) goto try_default;// 书本未加载
                founded_book = founded[book_name];
                string returns = founded_book.GetString(key, index);
                if (returns == null) goto try_default;
                if (returns == "") goto try_default;// 已加载的书中不存在
                return returns;
            }
        try_default:
            if (data.ContainsKey(language_default)/*尝试从默认语言中读取*/)
            {
                founded = data[language_default];
                if (founded.ContainsKey(book_name) == false) return null;
                founded_book = founded[book_name];
                return founded_book.GetString(key, index);
            }
            return null;
        }
        /// <summary>
        /// // 取出文本， code 格式为： book_name/key/0，最后一个参数可以省略，语言锁定为 language_using 。
        /// </summary>
        /// <param name="code"></param>
        public string DataGetQuick(string code)
        {
            string[] args = code.Split('/', '\\', ' ');
            string book_name, key; int index;
            if (args.Length < 2) return null;
            book_name = args[0]; key = args[1];
            try { if (args.Length == 2) { index = 0; } else index = System.Convert.ToInt32(args[2]); }
            catch { return null; }
            return DataGet(book_name, key, index);
        }
    }
}