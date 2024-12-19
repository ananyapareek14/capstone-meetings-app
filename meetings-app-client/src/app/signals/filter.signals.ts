// import { computed, signal } from '@angular/core';

// // type FilterFunction = <ItemType>(item: ItemType) => boolean;

// export const useFilterableData = <
//   // FilterKeyProperty extends string,
//   ItemType extends { name: string }
// >() =>
//   // filterFunction: FilterFunction
//   {
//     const array = signal([] as Array<ItemType>);
//     const filterKey = signal('');
//     const filteredArray = computed(() => {
//       const key = filterKey().toUpperCase();

//       return array().filter((item) => {
//         return item.name.toUpperCase().includes(key);
//       });
//     });

//     const hasFilteredOutItems = array().length !== filteredArray().length;

//     return {
//       array,
//       filterKey,
//       filteredArray,
//       hasFilteredOutItems,
//     };
//   };

import { computed, signal } from '@angular/core';

export const useFilterableData = <
  ItemType extends { description: string }
>() => {
  const meetings = signal([] as Array<ItemType>);
  const filterKey = signal('');
  const filteredMeetings = computed(() => {
    const key = filterKey().toLowerCase();
    return meetings().filter((item) => {
      return item.description.toLowerCase().includes(key);
    });
  });

  return {
    meetings,
    filterKey,
    filteredMeetings,
  };
};

