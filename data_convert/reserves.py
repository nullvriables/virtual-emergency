import json
from shapely.geometry import mapping, shape

j = json.loads(open("ACT Reserves.geojson",'r').read())
f = open('reserves.csv','w')

for i in j['features']:
	name = i['properties']['name']
	coords = shape(i['geometry']).centroid.coords[:]
	line = name + ',' + str(coords[0][0]) + ',' + str(coords[0][1])
	print(line)
	f.write(line + '\n')

f.close()
