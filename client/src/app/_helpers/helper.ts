export class Helper {
    
    /**
    * This is the arrays comparator function
    * @param predicate This is your func comparator for arrayes elems which shoud return boolean value
    * @param updaterFirstFn optional mapping function for first array's elements
    * @param updaterSecondFn optional mapping function for second array's elements
    * @returns returns a tuple of two arrays, where first elem of tuple is array with matched elems and second array with unmatched elems
    */
    static compareArrays<T>(first: T[], second: T[], predicate, updaterFirstFn?, updaterSecondFn?): [[], []] {
        const matchedElems = []
        const unmatchedElems = []
        return this.recursiveComparatorLel(matchedElems, unmatchedElems, first, second, 0, predicate, updaterFirstFn, updaterSecondFn)
        
        // for (let i = 0; i < first.length; i++) {
        //     let isMatched = false;
        //     for (let j = 0; j < second.length; j++) {
        //         if (predicate(first[i], second[j])){
        //             if (updaterFirstFn) {
        //                 matchedElems.push(updaterSecondFn(second[j]))
        //             } else {
        //                 matchedElems.push(second[j])
        //             }
                    
        //             isMatched = true;
        //             break;
        //         }
        //     }
        //     if (!isMatched) { 
        //         if (updaterFirstFn) {
        //             unmatchedElems.push(updaterFirstFn(first[i]))
        //         } else {
        //             unmatchedElems.push(first[i])
        //         }
                
        //     }
        // }
        // return [matchedElems, unmatchedElems];
    }
    static recursiveComparatorLel<T>(matchedArr: T[], unMatchedArr: T[], ar1: T[], ar2: T[], pointer: number, predicate, fn1, fn2) {
        if (ar1.length === 0) return [matchedArr, unMatchedArr]
        
        if (pointer > ar2.length) {
            return this.recursiveComparatorLel(matchedArr, [...unMatchedArr, fn1(ar1[0])], ar1.slice(1), ar2, 0, predicate, fn1, fn2)
        }
        if(predicate(ar1[0],ar2[pointer])) {
            return this.recursiveComparatorLel([...matchedArr, fn2(ar1[0])], unMatchedArr, ar1.slice(1), ar2, 0, predicate, fn1, fn2)
        }
       
        return this.recursiveComparatorLel(matchedArr, unMatchedArr, ar1, ar2, ++pointer, predicate, fn1, fn2)
    }
}
