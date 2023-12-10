"use client";
import { React, createContext, useState } from 'react'

const FilterContext = createContext({
    "蛋奶素": false,
    "肉類": false,
    "海鮮": false,
    "餐點時間": "Lunch"
});

function FilterProvider({ children }) {
    const [curFilterState, setFilterState] = useState({
        "蛋奶素": false,
        "肉類": false,
        "海鮮": false,
        "餐點時間": "Lunch"
    })

    const defaultValue = {
        curFilterState: curFilterState,
        setFilterState: setFilterState
    };

    return (
        <FilterContext.Provider value={defaultValue}>
            {children}
        </FilterContext.Provider>
    );
} 

export { FilterContext, FilterProvider };