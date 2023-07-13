read -p "Enter CompanyName: " CompanyName
read -p "Enter ProjectName: " ProjectName
read -p "Enter ServiceName: " ServiceName

NewVal="${CompanyName}.${ProjectName}.${ServiceName}";
AtOldVal="TWJ.TWJApp.TWJService";
UnderLineOldVal="TWJ.TWJApp.TWJService";
ServiceDbContext="Service@DbContext";

echo "Started Folder Replacement...";
find . -depth -type d -name "${AtOldVal}*" -exec sh -c 'x="{}"; DIR="$(dirname "${x}")" ; FOLDER="$(basename "${x}")"; mv "$x" "${DIR}/${FOLDER/'$AtOldVal'/'$NewVal'}";' \;
echo "--Finished Folder Replacement";

echo "---Started File Replacement...";
find . -type f -name "${AtOldVal}*" -exec sh -c 'x="{}"; DIR="$(dirname "${x}")" ; FILE="$(basename "${x}")"; mv "$x" "${DIR}/${FILE/'$AtOldVal'/'$NewVal'}";' \;
find . -type f -name "*${ServiceDbContext}*" -exec sh -c 'x="{}"; DIR="$(dirname "${x}")" ; FILE="$(basename "${x}")"; mv "$x" "${DIR}/${FILE/'$ServiceDbContext'/'$ProjectName'DbContext}";' \;
echo "----Finished File Replacement";

echo "-----Started File Content Replacement...";
find ./ -type f -exec sed -i -e 's/'$UnderLineOldVal'/'$NewVal'/g' {} \;
find ./ -type f -exec sed -i -e 's/'$AtOldVal'/'$NewVal'/g' {} \;
find ./ -type f -exec sed -i -e 's/TWJAppDbContext/'$ProjectName'DbContext/g' {} \;
echo "------Finished File Content Replacement";

echo "DONE";