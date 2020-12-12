using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace DataCollection.Utils
{
    public class AutoMapperHelper
    {
        private static IMapper _mapper = null;
        public static void InitMapper(string[] assemblys)
        {
            MapperConfiguration configuration = new MapperConfiguration(e => {
                e.AddMaps(assemblys);
            });
            _mapper = configuration.CreateMapper();
        }


        public static IMapper mapper
        {
            get {
                if (_mapper is null) throw new Exception("_mapper对象未初始化");
                return _mapper;
            }
        }
    }
}
