import { computed, signal } from '@angular/core';

// type FilterFunction = <ItemType>(item: ItemType) => boolean;

export const useFilterableData = <
  // FilterKeyProperty extends string,
  ItemType extends { name: string }
>() =>
  // filterFunction: FilterFunction
  {
    const array = signal([] as Array<ItemType>);
    const filterKey = signal('');
    const filteredArray = computed(() => {
      const key = filterKey().toUpperCase();

      return array().filter((item) => {
        return item.name.toUpperCase().includes(key);
      });
    });

    const hasFilteredOutItems = array().length !== filteredArray().length;

    return {
      array,
      filterKey,
      filteredArray,
      hasFilteredOutItems,
    };
  };