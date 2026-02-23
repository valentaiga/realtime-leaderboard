export const debounce = <TCallback>(callback: TCallback, delay: number) => {
  let timeout: number | null = null;

  return () => {
    if (timeout) {
      clearTimeout(timeout);
    }

    timeout = setTimeout(() => {
      (callback as () => void)();
      timeout = null;
    }, delay);
  };
};
