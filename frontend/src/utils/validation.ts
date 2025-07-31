export const isValidNip = (nip: string): boolean => {
  if (!nip) return false;
  nip = nip.replace(/[\s-]/g, "");
  if (nip.length !== 10 || !/^\d+$/.test(nip)) {
    return false;
  }
  const weights = [6, 5, 7, 2, 3, 4, 5, 6, 7];
  let sum = 0;
  for (let i = 0; i < 9; i++) {
    sum += parseInt(nip[i], 10) * weights[i];
  }
  const checksum = sum % 11;
  const controlDigit = parseInt(nip[9], 10);
  return checksum === controlDigit;
};
