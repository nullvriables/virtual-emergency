import json
from shapely.geometry import mapping, shape

j = json.loads(open("sport.geojson",'r').read())
f = open('sport.csv','w')

for i in j['features']:
	name = i['properties']['GROUND_NAME']
	type = i['properties']['GROUND_TYPE']
	division = i['properties']['DIVISION']
	label = ''
	if name != '':
		label = name
	else:
		label = division.title() + ' ' + type
	coords = shape(i['geometry']).centroid.coords[:]
	line = label + ',' + str(coords[0][0]) + ',' + str(coords[0][1])
	print(line)
	f.write(line + '\n')

f.close()
